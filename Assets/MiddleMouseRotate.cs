using UnityEngine;

public class MiddleMouseRotate : MonoBehaviour
{
    [Header("旋转控制设置")]
    [Tooltip("旋转速度系数")]
    public float rotationSpeed = 5.0f;
    
    [Tooltip("是否反转旋转方向")]
    public bool invertRotation = false;
    
    [Tooltip("是否启用平滑旋转")]
    public bool smoothRotation = true;
    
    [Tooltip("平滑旋转的插值速度")]
    public float smoothFactor = 10.0f;
    
    // 内部变量
    private Vector2 _previousMousePosition;
    private float _targetRotationY;
    private float _currentRotationY;

    void Start()
    {
        // 初始化旋转值为当前物体的Y轴旋转
        _targetRotationY = transform.eulerAngles.y;
        _currentRotationY = _targetRotationY;
    }

    void Update()
    {
        HandleMiddleMouseRotation();
    }

    void HandleMiddleMouseRotation()
    {
        // 检查鼠标中键是否被按住
        if (Input.GetMouseButton(2)) // 2代表鼠标中键
        {
            // 计算鼠标移动量
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 mouseDelta = currentMousePosition - _previousMousePosition;
            
            // 计算旋转量（考虑是否反转方向）
            float rotationAmount = mouseDelta.x * rotationSpeed * (invertRotation ? -1 : 1);
            
            // 更新目标旋转角度
            _targetRotationY += rotationAmount;
            
            // 确保角度在0-360范围内
            if (_targetRotationY > 360) _targetRotationY -= 360;
            if (_targetRotationY < 0) _targetRotationY += 360;
        }
        
        // 更新前一帧鼠标位置
        _previousMousePosition = Input.mousePosition;
        
        // 应用旋转
        ApplyRotation();
    }
    
    void ApplyRotation()
    {
        if (smoothRotation)
        {
            // 使用插值实现平滑旋转
            _currentRotationY = Mathf.LerpAngle(_currentRotationY, _targetRotationY, smoothFactor * Time.deltaTime);
        }
        else
        {
            // 直接设置旋转
            _currentRotationY = _targetRotationY;
        }
        
        // 应用旋转到物体（只改变Y轴旋转）
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, _currentRotationY, transform.eulerAngles.z);
    }
    
    // 可选：在Inspector中重置当前旋转
    [ContextMenu("重置旋转")]
    public void ResetRotation()
    {
        _targetRotationY = 0;
        _currentRotationY = 0;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 0, transform.eulerAngles.z);
    }
}