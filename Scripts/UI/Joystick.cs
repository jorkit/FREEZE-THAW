using Godot;
using System;
using FreezeThaw.Utils;
using System.Linq;

public partial class Joystick : Sprite2D
{
    private Sprite2D _point;// 声明一个Sprite2D类型的私有变量_point（圆点）
    private byte _maxlen;// 声明一个byte类型的私有变量maxlen并赋值为70
	private sbyte _index;
    private sbyte _ondraging;// 声明一个sbyte类型的私有变量并赋值为-1
    private UIContainer _uiContainer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Visible = false;
		_maxlen = 70;
		_index = 0;
		_ondraging = -1;
        _uiContainer = GetParentOrNull<UIContainer>();
        if (_uiContainer == null)
        {
            LogTool.DebugLogDump("UIContainer not found!");
        }
        _point = GetNodeOrNull<Sprite2D>("Point");
        if (_point == null)
        {
            LogTool.DebugLogDump("Point not found!");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
	{
        if (_uiContainer.character.GetType().BaseType.BaseType != typeof(Character))
        {
            AIRunning();
        }
    }

	public override void _Input(InputEvent @event)
	{
        if (NetworkControler.IsMultiplayer == true)
        {
            if (IsMultiplayerAuthority() == false || _uiContainer.character.GetType().BaseType.BaseType != typeof(Character))
            {
                return;
            }
        }
        else
        {
            if (_uiContainer.character != PlayerControler.Player)
            {
                return;
            }
        }
        
        if (XBOXJoystickHandle(@event) == true)
        {
            return;
        }
        if (KeyBoardDirectionHandle(@event) == true)
        {
            return;
        }
        JoystickTouchHandle(@event);
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
                _ondraging = _index;// 记录点触索引值防止多指影响触控
				_point.GlobalPosition = mousebutton;// 更新_point（圆点）的全局位置
				if (_point.Position.Length() > _maxlen)// 如果_point（圆点）的位置长度大于最大长度
				{
					_point.Position = _point.Position.Normalized() * _maxlen;// 将_point（圆点）的位置设置为单位向量乘以最大长度
				}
			}
		}
	}

	private void JoystickTouchHandle(InputEvent @event)
	{
        if (@event is InputEventScreenTouch && @event.IsPressed() && _ondraging == -1)
        {
            Vector2 tmp_vec = (Vector2)@event.Get("position");
            if (tmp_vec.X > UIControler.WindowSize.X/2 || tmp_vec.X < UIControler.WindowSize.X/15
                || tmp_vec.Y < UIControler.WindowSize.Y/8 || tmp_vec.Y > UIControler.WindowSize.Y * 9/10)
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
            _point.Position = Vector2.Zero;
            //CreateTween().TweenProperty(_point, "position", Vector2.Zero, 0.1).SetTrans(Tween.TransitionType.Linear);
        }
    }

    private bool XBOXJoystickHandle(InputEvent @event)
    {
        if (@event is InputEventJoypadMotion)
        {
            var x = Input.GetJoyAxis(0, JoyAxis.LeftX);

            var y = Input.GetJoyAxis(0, JoyAxis.LeftY);

            _point.Position = new Vector2(x, y).Normalized();
            return true;
        }

        return false;
    }

    private bool KeyBoardDirectionHandle(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            var velocity = new Vector2();
            var left = Input.IsActionPressed("ui_left");
            var right = Input.IsActionPressed("ui_right");
            var up = Input.IsActionPressed("ui_up");
            var down = Input.IsActionPressed("ui_down");

            if (left) velocity.X--;

            if (right) velocity.X++;

            if (up) velocity.Y--;

            if (down) velocity.Y++;

            left = Input.IsActionJustReleased("ui_left");
            right = Input.IsActionJustReleased("ui_right");
            up = Input.IsActionJustReleased("ui_up");
            down = Input.IsActionJustReleased("ui_down");

            if (left) velocity.X = 0;
 
            if (right) velocity.X = 0;

            if (up) velocity.Y = 0;

            if (down) velocity.Y = 0;

            if (velocity != Vector2.Zero)
            {
                _point.Position = velocity.Normalized();
                return true;
            }
            _point.Position = Vector2.Zero;
            return true;
        }

        return false;
    }

    /* 外部调用返回当前位置的方法 */
    public Vector2 GetCurPosition()
    {
        return _point.Position.Normalized(); // 返回_point位置的单位向量
    }

    private void AIRunning()
    {
        if (NetworkControler.IsMultiplayer == true && NetworkControler.MultiplayerApi.IsServer() == false)
        {
            return;
        }
        if (PlayerControler.Monster == null || _uiContainer.character == null)
        {
            LogTool.DebugLogDump("Translating");
            return;
        }
        if (_uiContainer.character.GetType().BaseType.BaseType == typeof(Monster))
        {
            AiFTBTrigger();
            /* Detect the survivor */
            if (_uiContainer.character.GetNodeOrNull<AIRadiusCheck>("AIRadiusCheck").SurvivorsInArea.Count <= 0)
            {
                _point.Position = Vector2.Zero;
                return;
            }
            var target = _uiContainer.character.GetNodeOrNull<AIRadiusCheck>("AIRadiusCheck").SurvivorsInArea.Find(survivor=>survivor.GetCurrentState() != CharacterStateEnum.Sealed);
            if (target ==  null)
            {
                _point.Position = Vector2.Zero;
                return;
            }
            /* chase the survivor */
            if (_uiContainer.character.Position.DistanceTo(target.Position) > 150)
            {
                _point.Position = (target.Position - _uiContainer.character.Position).Normalized();
            }
            /* attach the survivor */
            else
            {
                _point.Position = Vector2.Zero;
                var attackDirection = (target.Position - _uiContainer.character.Position).Normalized();
                if (attackDirection == Vector2.Zero)
                {
                    return;
                }
                AiATBTrigger(attackDirection);
            }
        }
        /* AI Survivor */
        else
        {
            /* Freezing or Sealed can do Nothing */
            if (_uiContainer.character?.GetCurrentState() == CharacterStateEnum.Freezing || _uiContainer.character?.GetCurrentState() > CharacterStateEnum.Freezed)
            {
                return;
            }
            /* Freeing other survivor */
            var survivor = _uiContainer.character.GetNodeOrNull<RadiusCheck>("RadiusCheck").SurvivorsInArea.Find(survivor => survivor.GetCurrentState() == CharacterStateEnum.Sealed);
            if (survivor != null)
            {
                AiFTBTrigger();
                return;
            }
            /* Freezed state */
            if (_uiContainer.character?.GetCurrentState() == CharacterStateEnum.Freezed)
            {
                AiFTBTrigger();
                return;
            }
            /* Freezing to protect self */
            if (_uiContainer.character?.Position.DistanceTo(PlayerControler.Monster.Position) < 150)
            {
                AiFTBTrigger();
                return;
            }
            /* control the distance to Monster large than 1000 */
            else if (_uiContainer.character?.Position.DistanceTo(PlayerControler.Monster.Position) < 1000)
            {
                _point.Position = (_uiContainer.character.Position - PlayerControler.Monster.Position).Normalized();
            }
            else if (_uiContainer.character?.Position.DistanceTo(PlayerControler.Monster.Position) > 1100)
            {
                _point.Position = (PlayerControler.Monster.Position - _uiContainer.character.Position).Normalized();
            }
            /* attack in safe distance */
            else
            {
                _point.Position = Vector2.Zero;
                var attackDirection = (PlayerControler.Monster.Position - _uiContainer.character.Position).Normalized();
                if (attackDirection == Vector2.Zero)
                {
                    return;
                }
                AiATBTrigger(attackDirection);
            }
        }
    }

    private void AiFTBTrigger()
    {
        var FTB = _uiContainer.GetNodeOrNull<FreezeThawButton>("FreezeThawButton");
        if (FTB == null)
        {
            LogTool.DebugLogDump("FTB not found!");
            return;
        }
        if (FTB.CanBePressed == true)
        {
            FTB.PressedHandle();
            return;
        }
    }

    private void AiATBTrigger(Vector2 direction)
    {
        var ATB = _uiContainer.GetNodeOrNull<AttackButton>("AttackButton");
        if (ATB == null)
        {
            LogTool.DebugLogDump("ATB not found!");
            return;
        }
        if (ATB.CanBePressed == true)
        {
            ATB.SetNewPosition(direction);
            ATB.ReleaseHandle();
        }
    }
}
