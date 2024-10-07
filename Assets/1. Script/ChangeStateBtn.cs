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
        stateText.text = "현재 아군 상태 : " + regularUnitBehaviour.regularStateName;
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
