using UnityEngine;

using State = StateMachine<Enemy>.State;

public class Enemy : MonoBehaviour
{
    // Enemyの総数
    public static int Count { get; private set; }

    private StateMachine<Enemy> stateMachine;

    private enum Event : int
	{
        // 時間切れ
        Timeout,
        // 残り1体になった
        LastOne,
	}

    private void Awake()
	{
        Count++;
	}

	private void OnDestroy()
	{
        Count--;
	}

	private void Start()
    {
        stateMachine = new StateMachine<Enemy>(this);

        // 残りが自分だけになったら動き出す
        stateMachine.AddTransition<StateStop, StateRandomWalk>((int)Event.LastOne);

        // 一定時間ごとに ランダムウォーク ⇔ ランダム待機
        stateMachine.AddTransition<StateRandomWalk, StateRandomWait>((int)Event.Timeout);
        stateMachine.AddTransition<StateRandomWait, StateRandomWalk>((int)Event.Timeout);

        stateMachine.Start<StateStop>();
    }

    private void Update()
    {
        stateMachine.Update();

        if (stateMachine.CurrentState is StateStop &&
            Count == 1)
		{
            stateMachine.Dispatch((int)Event.LastOne);
		}
    }

    // 何もしない
    private class StateStop : State
	{
    }

    // ランダム待機
    private class StateRandomWait : State
    {
        private const float TimeoutMin = 0.5f;
        private const float TimeoutMax = 1.0f;
        private float timeoutAt;

        protected override void OnEnter(State prevState)
        {
            timeoutAt = Time.time + Random.Range(TimeoutMin, TimeoutMax);
        }

        protected override void OnUpdate()
        {
            if (timeoutAt <= Time.time)
            {
                StateMachine.Dispatch((int)Event.Timeout);
            }
        }
    }

    // ランダムウォーク
    private class StateRandomWalk : State
    {
        private const float Speed = 4f;
        private const float TimeoutMin = 0.5f;
        private const float TimeoutMax = 1.0f;
        private float timeoutAt;

        protected override void OnEnter(State prevState)
        {
            timeoutAt = Time.time + Random.Range(TimeoutMin, TimeoutMax);

            Owner.transform.Rotate(0, 0, Random.Range(0, 360f));
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
}
