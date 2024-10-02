﻿using Godot;
using System;
using FreezeThaw.Utils;
using System.Reflection;
public partial class AttackButton : Sprite2D
{
    private bool CanBePressed;
    private Sprite2D _point;// 声明一个Sprite2D类型的私有变量_point（圆点）
    private int _maxlen;// 声明一个byte类型的私有变量maxlen并赋值为70
    private sbyte _index;
    private sbyte _ondraging;// 声明一个sbyte类型的私有变量并赋值为-1
    public Vector2 direction;
    private UIContainer _uiContainer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _maxlen = 70;
        _index = 0;
        _ondraging = -1;
        _uiContainer = GetParentOrNull<UIContainer>();
        if (_uiContainer == null)
        {
            LogTool.DebugLogDump("UIContainer not found!");
            return;
        }
        _point = GetNodeOrNull<Sprite2D>("Point");
        if (_point == null)
        {
            LogTool.DebugLogDump("Point not found!");
        }
        _point.Visible = false;
        CanBePressed = true;

        /* set the position according to WindowSize */
        Position = new Vector2(BigBro.windowSize.X * 12 / 15, BigBro.windowSize.Y * 3 / 4);
 
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_uiContainer.character.GetCurrentState() >= CharacterStateEnum.Attack)
        {
            CanBePressed = false;
        }
        else
        {
            CanBePressed = true;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (BigBro.IsMultiplayer == true)
        {
            if (CanBePressed == false || BigBro.MultiplayerApi.IsServer() == true || IsMultiplayerAuthority() == false)
            {
                return;
            }
        }
        else
        {
            if (CanBePressed == false || _uiContainer.character != BigBro.Player)
            {
                return;
            }
        }
        
        if (XBOXJoystickHandle(@event) == true)
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
            Vector2 mousePosition = (Vector2)@event.Get("position");// 获取鼠标位置
            float mousePositionToPoint = (mousePosition - Position).Length();// 计算点击位置与中心位置之间的距离
            /* 直接点击摇杆外部不反应 */
            if (mousePositionToPoint > _maxlen * Scale.Abs().X && _point.Position.Length() == 0)
            {
                return;
            }

            if (mousePositionToPoint <= _maxlen || _index == _ondraging) // 如果距离小于等于最大长度或者索引值相等
            {
                _ondraging = _index;// 记录点触索引值防止多指影响触控
                _point.GlobalPosition = mousePosition;// 更新_point（圆点）的全局位置
                if (_point.Position.Length() > _maxlen)// 如果_point（圆点）的位置长度大于最大长度
                {
                    _point.Position = _point.Position.Normalized() * _maxlen;// 将_point（圆点）的位置设置为单位向量乘以最大长度
                }
                /* set the Postion of BulletDirection Marker2D for bullet line drawing */
                _uiContainer.character.GetNodeOrNull<Polygon2D>("BulletDirection").Visible = true;
                _uiContainer.character.GetNodeOrNull<Polygon2D>("BulletDirection").Rotation = _point.Position.Angle();
            }
        }
    }

    private void JoystickTouchHandle(InputEvent @event)
    {
        if (@event is InputEventScreenTouch && @event.IsPressed() && _ondraging == -1)
        {
            Vector2 tmp_vec = (Vector2)@event.Get("position");
            /* 直接点击摇杆外部不反应 */
            if ((tmp_vec - Position).Length() > _maxlen * Scale.Abs().X)
            {
                return;
            }
            _point.Visible = true;
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
            ReleaseHandle();
            _point.Visible = false;
            _ondraging = -1;
            _point.Position = Vector2.Zero;
            /* BulletDirection hide */
            _uiContainer.character.GetNodeOrNull<Polygon2D>("BulletDirection").Visible = false;
        }
    }
    private bool XBOXJoystickHandle(InputEvent @event)
    {
        if (@event is InputEventJoypadMotion)
        {
            var x = Input.GetJoyAxis(0, JoyAxis.RightX);
            var y = Input.GetJoyAxis(0, JoyAxis.RightY);

            if (x != 0 || y != 0)
            {
                _point.Visible = true;
                _point.Position = new Vector2(x, y).Normalized() * _maxlen;
                /* set the Postion of BulletDirection Marker2D for bullet line drawing */
                _uiContainer.character.GetNodeOrNull<Polygon2D>("BulletDirection").Visible = true;
                _uiContainer.character.GetNodeOrNull<Polygon2D>("BulletDirection").Rotation = _point.Position.Angle();
            }
            else
            {
                _point.Visible = false;
                _point.Position = Vector2.Zero;
                _uiContainer.character.GetNodeOrNull<Polygon2D>("BulletDirection").Visible = false;
            }
        }
        var attack = Input.IsActionPressed("Attack");
        if (attack == true)
        {
            ReleaseHandle();
            _point.Visible = false;
            _point.Position = Vector2.Zero;
            _uiContainer.character.GetNodeOrNull<Polygon2D>("BulletDirection").Visible = false;
        }

        return false;
    }

    public void ReleaseHandle()
    {
        if (!CanBePressed || IsMultiplayerAuthority() == false)
        {
            return;
        }
        LogTool.DebugLogDump("ATB released!");
        direction = (_point.GlobalPosition - Position).Normalized();
        if (direction == Vector2.Zero)
        {
            return;
        }
        if (BigBro.IsMultiplayer == true)
        {
            if (BigBro.MultiplayerApi.IsServer() == false)
            {
                var rpcRes = Rpc("ReleaseHandlerRpc");
                if (rpcRes != Error.Ok)
                {
                    LogTool.DebugLogDump("ReleaseHandlerRpc Failed! " + rpcRes.ToString());
                }
            }
        }
        else
        {
            _uiContainer.character.AttackButtonPressedHandle();
        }
    }

    [Rpc(mode: MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    public void ReleaseHandlerRpc()
    {
        LogTool.DebugLogDump(GetMultiplayerAuthority().ToString() + " receive Attack CMD from " + BigBro.MultiplayerApi.GetRemoteSenderId());
        _uiContainer.character.AttackButtonPressedHandle();
    }
}
