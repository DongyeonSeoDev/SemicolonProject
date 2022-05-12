using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Enemy4Archer : Enemy
    {
        // TODO: 공격 방식 정하고 코드 작성
        //private EnemyCommand enemyAttackCommand;
        //private EnemyCommand enemyAttackPlayerCommand;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.isLongDistanceAttack = true;
            enemyData.chaseSpeed = 10f;
            enemyData.attackDelay = 1f;
            enemyData.minRunAwayTime = 2f; // TODO : 값 설정
            enemyData.maxRunAwayTime = 4f; // TODO : 값 설정
            enemyData.playerAnimationTime = 0.55f;
            enemyData.maxHP = 50;
            enemyData.hp = 50;

            enemyData.enemyMoveCommand = new EnemyRandomMoveCommand(enemyData, positionCheckData); // MoveCommand 생성
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            //enemyAttackPlayerCommand = new EnemyAttackPlayerCommand(transform, this, enemyData.attackPower);
            //enemyAttackCommand = new EnemyAttackCommand(this, enemyData.enemyObject.transform, enemyData.attackPower);
        }

        //public void EnemyAttack() // 애니메이션에서 실행 - 적 공격
        //{
        //    if (enemyData.eEnemyController == EnemyController.PLAYER)
        //    {
        //        enemyAttackPlayerCommand.Execute();
        //    }
        //    else if (enemyData.eEnemyController == EnemyController.AI)
        //    {
        //        enemyAttackCommand.Execute();
        //    }
        //}
    }
}