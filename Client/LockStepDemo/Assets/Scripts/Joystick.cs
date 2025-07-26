using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Joystick : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static Joystick Instance { get; private set; }

    public enum Type
    {
        Fixed,
        Floating,
    }

    public Type joystickType = Type.Fixed;
    [Range(0, 1)] public float deadZone = 0.2f;
    [Range(0, 1)] public float moveThreshold = 0.1f;

    public RectTransform background, handle;

    RectTransform _root;
    Canvas _canvas;
    Camera _cam;
    Vector2 _radius;
    bool _dragging;
    Vector2 _input;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _root = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _cam = (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
            ? _canvas.worldCamera
            : null;
        _radius = background.sizeDelta * 0.5f;
        if (joystickType != Type.Fixed)
            background.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (_dragging) return;
        _dragging = true;
        if (joystickType != Type.Fixed)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _root, e.position, _cam, out Vector2 pos);
            background.anchoredPosition = pos;
            background.gameObject.SetActive(true);
        }

        UpdateHandle(e.position);
    }

    public void OnDrag(PointerEventData e)
    {
        if (!_dragging) return;
        UpdateHandle(e.position);
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (!_dragging) return;
        _dragging = false;
        if (joystickType != Type.Fixed)
            background.gameObject.SetActive(false);
        _input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    void UpdateHandle(Vector2 screenPos)
    {
        Vector2 bgp = (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            ? (Vector2)background.position
            : RectTransformUtility.WorldToScreenPoint(_cam, background.position);

        Vector2 delta = (screenPos - bgp) / _canvas.scaleFactor;
        delta.x = Mathf.Clamp(delta.x, -_radius.x, _radius.x);
        delta.y = Mathf.Clamp(delta.y, -_radius.y, _radius.y);
        handle.anchoredPosition = delta;

        Vector2 raw = new Vector2(delta.x / _radius.x, delta.y / _radius.y);
        float mag = raw.magnitude;
        _input = mag < deadZone
            ? Vector2.zero
            : raw.normalized * Mathf.Min(mag, 1f);
    }

    public Vector2 Value => _input;
}