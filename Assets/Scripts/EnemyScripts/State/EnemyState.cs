namespace Enemy
{
    public partial class EnemyState // 利 惑怕 包府 何葛 内靛
    {
        public enum eState
        {
            IDLE, MOVE, CHASE, ATTACK, STUN, DEAD
        }

        public enum eEvent
        { 
            START, UPDATE, END
        }

        public eState stateName;
        protected eEvent currentEvent;

        protected EnemyState nextState;
        protected EnemyData enemyData;

        public EnemyState(eState state, EnemyData enemyData)
        {
            stateName = state;
            currentEvent = eEvent.START;
            this.enemyData = enemyData;
        }

        protected virtual void Start() => currentEvent = eEvent.UPDATE;
        protected virtual void Update() => StateChangeCondition();
        protected virtual void End() { }

        public EnemyState Process()
        {
            switch (currentEvent)
            {
                case eEvent.START:
                    Start();
                    break;
                case eEvent.UPDATE:
                    Update();
                    break;
                case eEvent.END:
                    End();
                    return nextState;
            }

            return this;
        }

        protected virtual void StateChangeCondition() { }

        protected void ChangeState(EnemyState state)
        {
            nextState = state;
            currentEvent = eEvent.END;
        }
    }
}