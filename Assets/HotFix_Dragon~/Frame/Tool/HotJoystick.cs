using HotGersonFrame.HotIlRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotGersonFrame.Tool
{
    public class HotJoystick : BaseHotMono, IPointerDown, IPointerDrag, IPointerUp,IStart
    {
        public HotJoystick(GameObject go) : base(go)
        {
            RegisterMonoEvt(go);
        }

        protected override void RegisterMonoEvt(GameObject go)
        {
            go.RegisterIMono<IStart>(this);
            go.RegisterIMono<IPointerDown>(this);
            go.RegisterIMono<IPointerDrag>(this);
            go.RegisterIMono<IPointerUp>(this);
            go.RegisterOver();
        }


        public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
        public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
        public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

        public Vector3 WorldDirection;

        public float HandleRange
        {
            get { return handleRange; }
            set { handleRange = Mathf.Abs(value); }
        }
        public float DeadZone
        {
            get { return deadZone; }
            set { deadZone = Mathf.Abs(value); }
        }

        public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }
        public bool SnapX { get { return snapX; } set { snapX = value; } }
        public bool SnapY { get { return snapY; } set { snapY = value; } }

        [SerializeField] protected float handleRange = 1;
        [SerializeField] protected float deadZone = 0;
        [SerializeField] protected AxisOptions axisOptions = AxisOptions.Both;
        [SerializeField] protected bool snapX = false;
        [SerializeField] protected bool snapY = false;

        [SerializeField] protected RectTransform background = null;
        [SerializeField] protected RectTransform handle = null;
        protected RectTransform baseRect = null;

        protected Canvas canvas;
        protected CanvasScaler scaler;
        protected Camera cam;

        protected Vector2 input = Vector2.zero;
        protected Vector2 m_lastfingerPos = Vector2.zero;

        public virtual void Start()
        {
            HandleRange = handleRange;
            DeadZone = deadZone;
            baseRect =gameObject.GetComponent<RectTransform>();
            canvas = gameObject.GetComponentInParent<Canvas>();
            scaler = canvas.GetComponent<CanvasScaler>();
            if (canvas == null)
                MyDebuger.LogError("The Joystick is not placed inside a canvas");

            Vector2 center = new Vector2(0.5f, 0.5f);
            background.pivot = center;
            handle.anchorMin = center;
            handle.anchorMax = center;
            handle.pivot = center;
            handle.anchoredPosition = Vector2.zero;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            input = Vector2.zero;
            this.m_lastfingerPos = eventData.position;
            background.anchoredPosition = new Vector2(scaler.referenceResolution.x / Screen.width * eventData.position.x, scaler.referenceResolution.y / Screen.height * eventData.position.y);
            //MyDebuger.Log(eventData.position + "\n" +  "anchoredPosition:" + background.anchoredPosition+"\n"+"screen"+Screen.width+","+Screen.height);
            handle.anchoredPosition = Vector2.zero;
        }




        public virtual void OnDrag(PointerEventData eventData)
        {
            cam = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;

            Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
            Vector2 radius = background.sizeDelta / 2;
            input = (eventData.position - position) / (radius * canvas.scaleFactor);
            FormatInput();
            HandleInput(input.magnitude, input.normalized, radius, cam);
            handle.anchoredPosition = input * radius * handleRange;
            this.m_lastfingerPos = eventData.position;

        }

        protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
        {
            if (magnitude > deadZone)
            {
                if (magnitude > 1)
                    input = normalised;
            }
            else
                input = Vector2.zero;
        }

        protected void FormatInput()
        {
            if (axisOptions == AxisOptions.Horizontal)
                input = new Vector2(input.x, 0f);
            else if (axisOptions == AxisOptions.Vertical)
                input = new Vector2(0f, input.y);
        }

        protected float SnapFloat(float value, AxisOptions snapAxis)
        {
            if (value == 0)
                return value;

            if (axisOptions == AxisOptions.Both)
            {
                float angle = Vector2.Angle(input, Vector2.up);
                if (snapAxis == AxisOptions.Horizontal)
                {
                    if (angle < 22.5f || angle > 157.5f)
                        return 0;
                    else
                        return (value > 0) ? 1 : -1;
                }
                else if (snapAxis == AxisOptions.Vertical)
                {
                    if (angle > 67.5f && angle < 112.5f)
                        return 0;
                    else
                        return (value > 0) ? 1 : -1;
                }
                return value;
            }
            else
            {
                if (value > 0)
                    return 1;
                if (value < 0)
                    return -1;
            }
            return 0;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;

        }

        protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
        {
            Vector2 localPoint = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
            {
                Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
                return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
            }
            return Vector2.zero;
        }

    }

    public enum AxisOptions { Both, Horizontal, Vertical }
}
