using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class PhaseOneBattleCardUiTests
{
    [Test]
    public void BattleCardButtonUsesArtWithoutEmbeddedTextOverlays()
    {
        GameObject controllerObject = new GameObject("Controller");
        GameObject handRoot = new GameObject("Hand Root", typeof(RectTransform));
        PhaseOnePrototypeController controller = controllerObject.AddComponent<PhaseOnePrototypeController>();
        CardData card = ScriptableObject.CreateInstance<CardData>();
        card.Name = "Strike";
        card.Cost = 1;
        card.Description = "Deal 6 damage";
        card.Damage = 6;
        card.Tint = Color.red;
        card.ArtPath = "Cards/card_strike_attack";

        MethodInfo method = typeof(PhaseOnePrototypeController).GetMethod("CreateBattleCardButton", BindingFlags.Instance | BindingFlags.NonPublic);
        Button button = (Button)method.Invoke(controller, new object[] { handRoot.transform, card });

        Text[] textLayers = button.GetComponentsInChildren<Text>(true);
        Assert.AreEqual(0, textLayers.Length);

        Object.DestroyImmediate(card);
        Object.DestroyImmediate(handRoot);
        Object.DestroyImmediate(controllerObject);
    }
}
