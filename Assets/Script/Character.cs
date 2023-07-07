using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RoomCombat {
    public class Character : MonoBehaviour {
        public float health =100;
        public float speed;
        public float attackDame = 10;
        public float cooldownAttack;
        public int countKill;

        public float GetHp() {
            return health;
        }
        public void SetHp(float hp) {
            health = hp;
        }
        public float GetSpeed() {
            return speed;
        }
        public void SetSpeed(float _speed) {
            speed = _speed;
        }
        public int GetCountKill() {
            return countKill;
        }
        public void SetCountKill() {
            ++countKill;
        }
        public virtual void Attack() {

        }
        public virtual void Move() { }
        public virtual void Dead() { }
    }
}