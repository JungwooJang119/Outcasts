﻿using UnityEngine;

/// <summary>
/// I will be honest, this state was a mess in the past, I just swept it all up
/// Your welcome future reader :)
/// </summary>
public class TinkerHeldState : State
{
    private Vector2 m_worldPosition;
    private float m_followingY;
    public TinkerHeldState(Pawn context, PawnStateFactory factory) : base(context, factory)
    {
        m_isRootState = true;
        m_animationName = "_gun";
        InitializeSubState();
    }

    public override void CheckSwitchState()
    {
        if (!((TinkerPawn)m_context).IsHeld)
        {
            SwitchState(m_factory.TinkerDefaultState());
        }
        if (m_context.IsJumping)
        {
            SwitchState(m_factory.TinkerDefaultState());
        }
    }

    public override void InitializeSubState()
    {
        SetSubState(m_factory.Grounded());
    }

    public override void EnterState()
    {
        m_context.CanMove = false;   
    }

    public override void ExitState()
    {
        // Be able to move right before tinker lets tinker go
        m_context.CanMove = true;
        GameManager.Instance.Ashe.IsLifting = false;
    }
}
