using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Boss2Centipede : Enemy
    {
        public Transform movePivot;

        private EnemyCommand enemyMoveCommand;
        private EnemyCommand enemySpecialAttackMoveCommand;
        private EnemyCommand attackMoveCommand;
        private EnemyCommand rushAttackReadyCommand;
        private EnemyCommand rushAttackCommand;

        [HideInInspector] public LayerMask whatIsWall;
        [HideInInspector] public float currentSpeed = 0f;

        void Start()
        {
            currentSpeed = 10f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SetCommands();
        }
        protected override void Update()
        {
            base.Update();
        }

        private void SetCommands()
        {
            enemyMoveCommand = new CentipedeBossMoveCommand(enemyData, movePivot, rb, this);
            
            enemyData.enemyMoveCommand = enemyMoveCommand;
        }
    }
}
