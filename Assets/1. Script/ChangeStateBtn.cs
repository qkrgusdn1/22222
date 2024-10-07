using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeStateBtn : MonoBehaviour
{
    public Unit unit;
    public TMP_Text stateText;
    RegularUnitBehaviour regularUnitBehaviour;

    private void Update()
    {
        stateText.text = "���� �Ʊ� ���� : " + regularUnitBehaviour.regularStateName;
    }

    public void ChangeState(RegularUnitBehaviour regularUnitBehaviour)
    {
        this.regularUnitBehaviour = regularUnitBehaviour;
    }

    public void OnClickedChangeState(string regularStateBtnName)
    {
        regularUnitBehaviour.OnClickedChangeRegularUnitStateBtn(regularStateBtnName);
    }
}
