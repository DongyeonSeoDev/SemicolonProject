using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyData
    {
        public EnemyData(EnemyDataSO enemyDataSO)
        {
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

        public EnemyController eEnemyController = EnemyController.AI;
        public EnemyType enemyType;

        public Dictionary<EnemyAnimationType, int> animationDictionary = new Dictionary<EnemyAnimationType, int>();
        public List<EnemyLootData> enemyLootList;

        public Action attackTypeCheckCondition = null;
        public Action endAttack = null;
        public Action deadEvent = null;

        public Func<EnemyState> enemyChaseStateChangeCondition = null;
        public Func<EnemyState> addAIAttackStateChangeCondition = null;
        public Func<EnemyState> addChangeAttackCondition = null;

        public GameObject enemyObject;
        public GameObject enemyCanvas;
        public Animator enemyAnimator;
        public SpriteRenderer enemySpriteRenderer;
        public Rigidbody2D enemyRigidbody2D;
        public Enemy enemy;

        public EnemyCommand enemyMoveCommand;
        public EnemyCommand enemySpriteRotateCommand;
        public EnemyCommand playerControllerMove;

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
        public float isMaxAttackPlayerDistance = 10f;
        public float isRunAwayDistance = 9f;
        public float attackDelay = 1f;
        public float damageDelay = 0.1f;
        public float rushForce = 50f;
        public float stunTime = 0f;
        public float minRunAwayTime;
        public float maxRunAwayTime;
        public float currentRunAwayTime = 1f;
        public float playerAnimationTime;
        public float playerAnimationDelay = 0.5f;
        public float playerAnimationSpeed = 1.2f;
        public float startAttackDelay = 0f;
        public float noAttackTime = 2f;
        public float attackPower = 10;
        public float damagedValue;
        public float maxHP = 30;
        public float hp = 30;

        public bool isDamaged = false;
        public bool isAttack = false;
        public bool isLongDistanceAttack = false;
        public bool isRunAway = false;
        public bool isEnemyMove = false;
        public bool isKnockBack = false;
        public bool isPlayerControllerMove = false;
        public bool isUseKnockBack;
        public bool isNoKnockback = false;
        public bool isNoStun = false;
        public bool isNoAttack = false;
        public bool isParrying = false;
        public bool isUseIdleAnimation = false;
        public bool isMovePositionReset = false;
        public bool isMoveStop = false;

        public int minAttackCount = 0;
        public int maxAttackCount = 0;
    }
}