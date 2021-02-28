using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using State = StateMachine<Actor>.State;

public class Actor : MonoBehaviour
{
	private StateMachine<Actor> stateMachine;

	private enum Event : int
	{
		// 時間切れ
		Timeout,
		// 敵を見つけた
		FindEnemy,
		// 敵を倒した
		DefeatEnemy,
		// 敵が全て死んだ
		DefeatAllEnemy,
	}

	private void Start()
	{
		stateMachine = new StateMachine<Actor>(this);

		// 敵を見つけたら回転を終了して前進する
		stateMachine.AddTransition<StateRotation, StateMoveForward>((int)Event.FindEnemy);
		// 敵を倒したら前進を終了。回転に戻る
		stateMachine.AddTransition<StateMoveForward, StateRotation>((int)Event.DefeatEnemy);
		// 一定の時間、前進しても敵が見つからなかった場合も回転に戻る
		stateMachine.AddTransition<StateMoveForward, StateRotation>((int)Event.Timeout);

		// 敵が全て死んだら終了
		stateMachine.AddAnyTransition<StateEnd>((int)Event.DefeatAllEnemy);

		stateMachine.Start<StateRotation>();
	}

	private void Update()
	{
		if (Enemy.Count == 0)
		{
			stateMachine.Dispatch((int)Event.DefeatAllEnemy);
			return;
		}

		stateMachine.Update();

		// 前方にレイを飛ばして敵がいるか調べる
		var ray = new Ray(transform.position, transform.up);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			if (hit.transform.GetComponent<Enemy>())
			{
				// 敵を見つけた
				stateMachine.Dispatch((int)Event.FindEnemy);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.TryGetComponent(out Enemy enemy))
		{
			Destroy(enemy.gameObject);
			stateMachine.Dispatch((int)Event.DefeatEnemy);
		}
	}

	// 対象の方向へ回転する
	private class StateRotation : State
	{
		// 回転の速度
		private const float Speed = 90f;
		// 回転の方向
		private float direction;

		protected override void OnEnter(State prevState)
		{
			// ランダムで右回転するか、左回転するかを決める
			direction = Random.Range(0, 2) == 0 ? 1 : -1;
		}

		protected override void OnUpdate()
		{
			Owner.transform.Rotate(0, 0, Speed * direction * Time.deltaTime);
		}
	}

	// 前進する
	private class StateMoveForward : State
	{
		private const float Speed = 2f;
		private const float Timeout = 1f;
		private float timeoutAt;

		protected override void OnEnter(State prevState)
		{
			timeoutAt = Time.time + Timeout;
		}

		protected override void OnUpdate()
		{
			Owner.transform.position += Owner.transform.up * Speed * Time.deltaTime;

			if (timeoutAt <= Time.time)
			{
				StateMachine.Dispatch((int)Event.Timeout);
			}
		}
	}

	// 終了
	private class StateEnd : State
	{
		protected override void OnEnter(State prevState)
		{
			Owner.enabled = false;
		}
	}
}
