using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeStateBtn : MonoBehaviour
{
    public Unit unit;
    public TMP_Text stateText;
    RegularUnitBehaviour regularUnitBehaviour;

    private void Start()
    {
        regularUnitBehaviour = unit.GetComponent<RegularUnitBehaviour>();
    }

    private void Update()
    {
        stateText.text = "현재 아군 상태 : " + regularUnitBehaviour.regularStateName;
    }

    public void OnClickedChangeState(string regularStateBtnName)
    {
        regularUnitBehaviour.OnClickedChangeRegularUnitStateBtn(regularStateBtnName);
    }
}
