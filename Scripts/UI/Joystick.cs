using Godot;
using System;

public partial class Joystick : Sprite2D
{
    private static Sprite2D _point;// 声明一个Sprite2D类型的私有变量_point（圆点）
	private Character _character;
    private byte _maxlen;// 声明一个byte类型的私有变量maxlen并赋值为70
	private sbyte _index;
    private sbyte _ondraging;// 声明一个sbyte类型的私有变量并赋值为-1

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Visible = false;
		_maxlen = 70;
		_index = 0;
		_ondraging = -1;
        _point = GetNodeOrNull<Sprite2D>("Point");
        if (_point == null)
        {
            GD.Print("Point not found!");
        }
        _character = Manager.Player;
        if (_character == null)
        {
            GD.Print("Character not found!");
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
        JoystickPreprocess(@event);
        if (@event is InputEventScreenDrag)
		{
            _index = (sbyte)@event.Get("index");
            /* 其他手指不反应 */
            if (_index != _ondraging)
            {
                return;
            }
            Vector2 mousebutton = (Vector2)@event.Get("position");// 获取鼠标位置
            float mouseposition = (mousebutton - Position).Length();// 计算点击位置与中心位置之间的距离
			/* 直接点击摇杆外部不反应 */
			if (mouseposition > _maxlen && _point.Position.Length() == 0)
			{
				return;
			}
			
			if (mouseposition <= _maxlen || _index == _ondraging) // 如果距离小于等于最大长度或者索引值相等
			{
                FSMState.CharacterStateChange(_character, FSMState.CharacterStateEnum.Run);
                _ondraging = _index;// 记录点触索引值防止多指影响触控
				_point.GlobalPosition = mousebutton;// 更新_point（圆点）的全局位置
				if (_point.Position.Length() > _maxlen)// 如果_point（圆点）的位置长度大于最大长度
				{
					_point.Position = _point.Position.Normalized() * _maxlen;// 将_point（圆点）的位置设置为单位向量乘以最大长度
				}
			}
		}
	}

	private void JoystickPreprocess(InputEvent @event)
	{
        if (@event is InputEventScreenTouch && @event.IsPressed() && _ondraging == -1)
        {
            Vector2 tmp_vec = (Vector2)@event.Get("position");
            if (tmp_vec.X > Manager.windowSize.X/2 || tmp_vec.X < Manager.windowSize.X/15 || tmp_vec.Y < Manager.windowSize.Y/8 || tmp_vec.Y > Manager.windowSize.Y * 9/10)
            {
                return;
            }
            Position = (Vector2)@event.Get("position");
            Visible = true;
			_index = (sbyte)@event.Get("index");
            _ondraging = _index;
        }
        else if (@event is InputEventScreenTouch && !@event.IsPressed())
        {
			_index = (sbyte)@event.Get("index");
            if (_ondraging != _index)
            {
                return;
            }
            Visible = false;
            _ondraging = -1;
            CreateTween().TweenProperty(_point, "position", new Vector2(0, 0), 0.1).SetTrans(Tween.TransitionType.Linear);
            FSMState.CharacterStateChange(_character, FSMState.CharacterStateEnum.Idle);
        }
    }

    /* 外部调用返回当前位置的方法 */
    public static Vector2 GetCurPosition()
    {
        return _point.Position.Normalized(); // 返回_point位置的单位向量
    }
}
