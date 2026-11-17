using UnityEngine;
using UnityEngine.UI;
public class CrosshairCursor : MonoBehaviour
{
    [SerializeField] private RectTransform _crosshair;
    [SerializeField] private Canvas _canvas;

    private void Start()
    {
        Cursor.visible = false;                      // Ẩn con trỏ hệ thống
        Cursor.lockState = CursorLockMode.Confined;  // Nhốt trong Game window
    }


}
