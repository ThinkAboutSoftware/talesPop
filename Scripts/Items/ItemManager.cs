using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TalesPop.Datas;



namespace TalesPop.Objects.Items
{
    using static Common;

    public class ItemManager
    {
        private static readonly MainContainer<int, Item> popContainer = new MainContainer<int, Item>(GetUID, GetGroupId, GetChildrenId);
        private readonly Stack<Inventory> processInventory;
        private Inventory currentRootInventory;
        private Factory factory;



        public ItemManager(Factory factory = null)
        {
            this.factory = factory ?? new Normal();
            processInventory = new Stack<Inventory>();
        }

        public Inventory CreateInventory(string json)
        {
            JObject jObject = JObject.Parse(json);
            string typeString = jObject[ItemArgs.itemType].Value<string>();
            ItemType type = StringToEnum<ItemType>(typeString);

            currentRootInventory = null;

            if (type.Equals(ItemType.Pouch) || type.Equals(ItemType.ExtraPouch))
                return CreateInventory(type, jObject);

            return null;
        }

        /*
         * Behaviours
         */
        public Item SearchItem(int itemUID)
        {
            return popContainer.Search(itemUID);
        }

        public Item SearchItem(int inventoryUID, int itemUID)
        {
            return popContainer.Search(inventoryUID, itemUID);
        }

        public Factory Factory
        {
            get { return factory; }
            set { factory = value; }
        }

        public int Size => popContainer.Count;

        public void Clear() => popContainer.Clear();

        public void Remove(int uid) => popContainer.Remove(uid);

        public void Add(Item item) => popContainer.Add(item.uid, item);
        /*
         * Privates
         */
        private Inventory CreateInventory(ItemType type, JObject jObject)
        {
            int uid = jObject[ObjectArgs.uid].Value<int>();
            MirrorContainer<int, Item> mirrorContainer = popContainer.GenerateMirrorContainer(uid);
            Inventory currentInventory = factory.Create(type, jObject, mirrorContainer) as Inventory;

            if (currentRootInventory == null)
            {
                popContainer.Add(currentInventory.uid, currentInventory);
                currentRootInventory = currentInventory;
            }

            processInventory.Push(currentInventory);

            foreach (JToken element in currentInventory.contents)
            {
                if (CreateItem(element) == null)
                {
                    processInventory.Pop();
                    return null;
                }
            }

            return processInventory.Pop();
        }

        private Item CreateItem(JToken token)
        {
            JObject  jObject    = (JObject)token;
            ItemType itemType   = StringToEnum<ItemType>(jObject[ItemArgs.itemType].Value<string>());
            Inventory inventory = processInventory.Peek();
            Item     item       = IsSame(ItemType.Pouch, itemType) || IsSame(ItemType.ExtraPouch, itemType)
                                        ? CreateInventory(itemType, jObject)
                                        : factory.Create(itemType, jObject);
            int?     slotId     = inventory.EmptySlotId(item?.slotId);

            if (slotId == null || item == null || inventory?.Space == 0)
                return null;

            item.groupId = inventory.uid;
            item.slotId = (int)slotId;
            item.remove = RemoveDelegate;
            item.searchBag = SearchItem;
            inventory.Add(item);

            return item;
        }

        private void RemoveDelegate(int key, int uid)
        {
            Debug.Log($"key = {key}, uid = {uid}");

            if (popContainer[key] is Inventory inventory)
                inventory.Remove(uid);
        }

        private static int GetUID(Item item)
        {
            return item.uid;
        }

        private static int GetGroupId(Item item)
        {
            return item.groupId;
        }

        private static int[] GetChildrenId(Item item)
        {
            if (item is Inventory inventory)
            {
                return inventory.KeyArray;
            }

            return null;
        }

        /*
         * TEST_CODE
         */
        public IReadOnlyDictionary<int, Item> CONTAINER() => popContainer.C;
        public void SHOW_BAG_CONTENTS(int key)
        {

            if (SearchItem(key) is Inventory inventory)
            {
                Debug.LogWarning($"SHOW BAG CONTENTS -- inventory [uid = {key}] [name = {inventory.name}]");
                foreach (KeyValuePair<int, Item> pair in inventory.CONTAINER)
                    Debug.Log($"item = {pair.Value.name}, uid = {pair.Value.uid}");
            }
        }
        public MainContainer<int, Item> POP_CONTAINER() => popContainer;
    }
}