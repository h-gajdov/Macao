using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    Card prevSelected;

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length == 0) {
            if(prevSelected != null) {
                prevSelected.hovered = false;
                prevSelected = null;
            }
            return;
        }

        if (prevSelected != null) prevSelected.hovered = false;

        prevSelected = GetTopCardOfHits(hits);
        prevSelected.hovered = true;
    }

    private Card GetTopCardOfHits(RaycastHit[] hits) {
        Card selectedCard = hits[0].transform.GetComponent<Card>();
        foreach (RaycastHit raycastHit in hits) {
            Card card = raycastHit.transform.GetComponent<Card>();
            if (card == null) continue;
            if (selectedCard.transform.GetSiblingIndex() < card.transform.GetSiblingIndex()) selectedCard = card;
        }
        return selectedCard;
    }
}
