using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GersonFrame.Tool
{

    public class ButtonAmTool : MonoBehaviour
    {
        private EventTrigger m_eventTrigger;

        [Header("手指抬起大小")]
        public Vector3 m_PointUpScal = Vector3.one;
        [Header("手指按下大小")]
        public Vector3 m_PointDownScal = new Vector3(0.83f,0.83f,0.83f);
        [Header("动画时长")]
        public float m_AmDuring = 0.2f;


        // Start is called before the first frame update
        void Start()
        {
            m_eventTrigger = gameObject.GetCompententOrNew<EventTrigger>();

            var entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener(this.OnPointerUp);
            m_eventTrigger.triggers.Add(entryUp);

            var entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener(this.OnPointerDown);
            m_eventTrigger.triggers.Add(entryDown);
        }

        

        private void OnPointerDown(BaseEventData arg0)
        {
            transform.DOKill();
            transform.DOScale(m_PointDownScal, 0.2f);
        }

        private void OnPointerUp(BaseEventData arg0)
        {
            transform.DOKill();
            transform.DOScale(m_PointUpScal, 0.2f);
        }


        private void OnDestroy()
        { 
            transform.DOKill();
        }



        public void SumalteClick(System.Action onclick )          
        {
            transform.DOKill();
            transform.DOScale(m_PointDownScal, 0.2f).onComplete = () =>
            {
                onclick?.Invoke();
                OnPointerUp(null);
            };
        }

    }
}
