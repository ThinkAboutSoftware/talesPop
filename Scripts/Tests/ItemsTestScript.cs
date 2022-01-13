using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TalesPop.Items;




public class ItemsTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        Debug.LogWarning("--- TEST BEGIN ---");
        // Use the Assert class to test conditions

        string armorJson = "{" +
            "\"uid\": 0," +
            "\"name\": \"some named armor\", " +
            "\"nameId\": 0, " +
            "\"itemType\": \"Armor\", " +
            "\"capacity\": 10" +
            "}";

        string potionJson1 = "{" +
            "\"uid\": 2," +
            "\"name\": \"some named potion1\", " +
            "\"nameId\": 2, " +
            "\"itemType\": \"Potion\", " +
            "\"capacity\": 10" +
            "}";

        string potionJson2 = "{" +
            "\"uid\": 3," +
            "\"name\": \"some named potion2\", " +
            "\"nameId\": 3, " +
            "\"itemType\": \"Potion\", " +
            "\"capacity\": 10" +
            "}";

        string bagJson = "{" +
            "\"uid\": 1," +
            "\"name\": \"some named bag\", " +
            "\"nameId\": 1, " +
            "\"itemType\": \"Bag\", " +
            "\"capacity\": 10," +
            $"\"contents\": [{armorJson}, {potionJson1}, {potionJson2}]" +
            "}";

        ItemManager itemManager = new ItemManager();
        Item bag = itemManager.CreateBag(bagJson);

        Debug.LogWarning("--- TEST END ---");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    // [UnityTest]
    // public IEnumerator NewTestScriptWithEnumeratorPasses()
    // {
    //     // Use the Assert class to test conditions.
    //     // Use yield to skip a frame.
    //     yield return null;
    // }
}