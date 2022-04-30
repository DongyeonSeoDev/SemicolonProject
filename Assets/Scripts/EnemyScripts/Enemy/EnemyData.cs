using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class EnemyData
    {
        public Action adk = null;
        public EnemyData(EnemyDataSO enemyDataSO)
        {
            Test(() =>
            {
                Debug.Log("afdfdfdf");
            });
            enemyType = enemyDataSO.enemyType;
            normalColor = enemyDataSO.normalColor;
            damagedColor = enemyDataSO.damagedColor;
            enemyDeadEffectColor = enemyDataSO.enemyDeadEffectColor;
            playerNormalColor = enemyDataSO.playerNormalColor;
            playerDamagedColor = enemyDataSO.playerDamagedColor;
            playerDeadEffectColor = enemyDataSO.playerDeadEffectColor;

            EnemyManager.SetEnemyAnimationDictionary(animationDictionary, enemyDataSO.animationList);

            playerControllerMove = new EnemyMovePlayerControllerCommand(this);
        }

        private void Test(Action dk)
        {
            Debug.Log("aaaa");
        }
        public EnemyController eEnemyController = EnemyController.AI;
        public EnemyType enemyType;

        public Dictionary<EnemyAnimationType, int> animationDictionary = new Dictionary<EnemyAnimationType, int>();
        public List<EnemyLootData> enemyLootList;

        public Action attackTypeCheckCondition = null;
        public Func<EnemyState> enemyChaseStateChangeCondition = null;
        public Func<EnemyState> addAIAttackStateChangeCondition = null;
        public Func<EnemyState> addChangeAttackCondition = null;

        public GameObject enemyObject;
        public Animator enemyAnimator;
        public SpriteRenderer enemySpriteRenderer;
        public Rigidbody2D enemyRigidbody2D;

        public EnemyCommand enemyMoveCommand;
        public EnemyCommand enemySpriteRotateCommand;
        public EnemyCommand playerControllerMove;

        public Vector2? knockBackDirection;
        public Vector2 moveVector;

        public Color normalColor;
        public Color damagedColor;
        public Color enemyDeadEffectColor;

        public Color playerNormalColor;
        public Color playerDamagedColor;
        public Color playerDeadEffectColor;

        public float chaseSpeed = 5f;
        public float isAttackPlayerDistance = 2f;
        public float isMinAttackPlayerDistance = 5f;
        public float isRunAwayDistance = 9f;
        public float isMaxAttackPlayerDistance = 10f;
        public float attackDelay = 1f;
        public float damageDelay = 0.1f;
        public float rushForce = 50f;
        public float knockBackPower;
        public float stunTime = 0f;
        public float minRunAwayTime;
        public float maxRunAwayTime;
        public float currentRunAwayTime = 1f;
        public float playerAnimationTime;
        public float playerAnimationDelay = 1f;

        public bool isDamaged = false;
        public bool isAttack = false;
        public bool isAttackCommand = false;
        public bool isLongDistanceAttack = false;
        public bool isRunAway = false;
        public bool isEnemyMove = false;
        public bool isKnockBack = false;
        public bool isCurrentAttackTime = false;
        public bool isUseDelay = false;
        public bool isPlayerControllerMove = false;
        public bool isUseKnockBack;
        public bool isNoKnockback = false;
        public bool isNoStun = false;

        public int attackPower = 10;
        public int damagedValue;
        public int maxHP = 30;
        public int hp = 30;
    }
}