using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Enemy4Archer : Enemy
    {
        // TODO: ���� ��� ���ϰ� �ڵ� �ۼ�
        //private EnemyCommand enemyAttackCommand;
        //private EnemyCommand enemyAttackPlayerCommand;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.isLongDistanceAttack = true;
            enemyData.chaseSpeed = 10f;
            enemyData.attackDelay = 1f;
            enemyData.minRunAwayTime = 2f; // TODO : �� ����
            enemyData.maxRunAwayTime = 4f; // TODO : �� ����
            enemyData.playerAnimationTime = 0.55f;
            enemyData.maxHP = 50;
            enemyData.hp = 50;

            enemyData.enemyMoveCommand = new EnemyRandomMoveCommand(enemyData, positionCheckData); // MoveCommand ����
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            //enemyAttackPlayerCommand = new EnemyAttackPlayerCommand(transform, this, enemyData.attackPower);
            //enemyAttackCommand = new EnemyAttackCommand(this, enemyData.enemyObject.transform, enemyData.attackPower);
        }

        //public void EnemyAttack() // �ִϸ��̼ǿ��� ���� - �� ����
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