using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GersonFrame;
using UnityEngine.UI;
using System;



public class ClickEvent : ITypeEvent
{
    public GersonFrame.EventType mEventType => GersonFrame.EventType.Normal;
}

public class TestEventCommond : AbstractCommand
{
    public override void OnExecute(object arg1, object arg2, object arg3)
    {
        this.Architecture.SendEvt<ClickEvent>();
    }
}


public class EventBtn : MonoBehaviour,IController
{
    public IArchitecture Architecture => EventArchiteure.Interface;

    // Start is called before the first frame update
    void Start()
    {
        Button evtbtn=   this.gameObject.GetComponent<Button>();
        evtbtn.onClick.AddListener(this.OnBtnClick);
        this.Architecture.RegistEvt<ClickEvent>( OnClickCallBack);
    }

    private void OnClickCallBack(ClickEvent callbackdata)
    {
        Debug.Log(callbackdata.mEventType);
        this.Architecture.UnRegisterEvt<ClickEvent>(OnClickCallBack);
    }

    private void OnBtnClick()
    {
        this.SendCommand<TestEventCommond>();
    }


}
