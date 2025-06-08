using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStackManager : MonoBehaviour
{
    public static Stack<string> UndealtCards = new Stack<string>();
    public SpriteRenderer spriteRenderer;
    public Color selectedColor;
    public float smoothness = 5f;

    private void OnMouseEnter() {
        StopAllCoroutines();
        StartCoroutine(LerpToColor(selectedColor));
    }

    private void OnMouseOver() {
        if(Input.GetMouseButtonDown(0) && UndealtCards.Count > 0) {
            Debug.Log(UndealtCards.Peek());
            Card card = GameManager.PlayerOnTurn.cardArranger.SpawnCard(UndealtCards.Pop());
            if (UndealtCards.Count == 0) spriteRenderer.gameObject.SetActive(false);
            card.transform.position = transform.position;
        }
    }

    private void OnMouseExit() {
        StopAllCoroutines();
        StartCoroutine(LerpToColor(Color.white));
    }

    private IEnumerator LerpToColor(Color target) {
        while(spriteRenderer.color != target) {
            Color currentColor = spriteRenderer.color;
            spriteRenderer.color = Color.Lerp(currentColor, target, smoothness * Time.deltaTime);
            yield return null;
        }
    }

    public static void SetUndealtCards(List<string> listOfUndealtCards) {
        UndealtCards.Clear();
        listOfUndealtCards.Reverse();
        foreach(string card in listOfUndealtCards) {
            UndealtCards.Push(card);
        }
    }
}
