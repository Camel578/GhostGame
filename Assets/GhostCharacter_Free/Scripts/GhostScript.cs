using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    public class GhostScript : MonoBehaviour
    {
        private Animator Anim;
        private CharacterController _characterController;
        private Vector3 MoveDirection = Vector3.zero;
        // Cache hash values
        private static readonly int IdleState = Animator.StringToHash("Base Layer.idle");
        private static readonly int MoveState = Animator.StringToHash("Base Layer.move");
        private static readonly int SurprisedState = Animator.StringToHash("Base Layer.surprised");
        private static readonly int AttackState = Animator.StringToHash("Base Layer.attack_shift");
        private static readonly int DissolveState = Animator.StringToHash("Base Layer.dissolve");
        private static readonly int AttackTag = Animator.StringToHash("Attack");
        // dissolve
        [SerializeField] private SkinnedMeshRenderer[] MeshR;
        private float _invincibilityTimer = 0f;
        private float Dissolve_value = 1;
        private bool DissolveFlg = false;
        private const int maxHP = 3;
        private bool isDead;
        private Text HP_text;

        [SerializeField] private Transform _respawn;
        // moving speed
        [SerializeField] private float Speed = 4;

        void Start()
        {
            Anim = this.GetComponent<Animator>();
            _characterController = this.GetComponent<CharacterController>();

        }




        void Update()
        {
            STATUS();
            GRAVITY();
            if (_invincibilityTimer > 0f)
            {
                _invincibilityTimer -= Time.deltaTime;
            }
            // this character status
            if (!PlayerStatus.ContainsValue(true))
            {
                MOVE();
                //PlayerAttack();
            }
            else if (PlayerStatus.ContainsValue(true))
            {
                int status_name = 0;
                foreach (var i in PlayerStatus)
                {
                    if (i.Value == true)
                    {
                        status_name = i.Key;
                        break;
                    }
                }
                if (status_name == Dissolve)
                {
                    PlayerDissolve();
                }
                //else if (status_name == Attack)
                //{
                //    PlayerAttack();
                //}
                else if (status_name == Surprised)
                {
                    // nothing method
                }
            }
            // Dissolve
            if (isDead && !DissolveFlg)
            {
                Anim.CrossFade(DissolveState, 0.1f, 0, 0);
                DissolveFlg = true;
            }
            // processing at respawn
            else if (!isDead && DissolveFlg)
            {
                DissolveFlg = false;
            }
        }

        //---------------------------------------------------------------------
        // character status
        //---------------------------------------------------------------------
        private const int Dissolve = 1;
        private const int Attack = 2;
        private const int Surprised = 3;
        private Dictionary<int, bool> PlayerStatus = new Dictionary<int, bool>
    {
        {Dissolve, false },
        {Attack, false },
        {Surprised, false },
    };
        //------------------------------
        private void STATUS()
        {
            // during dissolve
            if (DissolveFlg && isDead)
            {
                PlayerStatus[Dissolve] = true;
            }
            else if (!DissolveFlg)
            {
                PlayerStatus[Dissolve] = false;
            }
            // during attacking
            if (Anim.GetCurrentAnimatorStateInfo(0).tagHash == AttackTag)
            {
                PlayerStatus[Attack] = true;
            }
            else if (Anim.GetCurrentAnimatorStateInfo(0).tagHash != AttackTag)
            {
                PlayerStatus[Attack] = false;
            }
            // during damaging
            if (Anim.GetCurrentAnimatorStateInfo(0).fullPathHash == SurprisedState)
            {
                PlayerStatus[Surprised] = true;
            }
            else if (Anim.GetCurrentAnimatorStateInfo(0).fullPathHash != SurprisedState)
            {
                PlayerStatus[Surprised] = false;
            }
        }
        // dissolve shading
        private void PlayerDissolve()
        {
            Dissolve_value -= Time.deltaTime;
            for (int i = 0; i < MeshR.Length; i++)
            {
                MeshR[i].material.SetFloat("_Dissolve", Dissolve_value);
            }
            if (Dissolve_value <= 0)
            {
                _characterController.enabled = false;
                Respawn();
            }
        }
        // play a animation of Attack
        //private void PlayerAttack()
        //{
        //    if (Input.GetKeyDown(KeyCode.A))
        //    {
        //        Anim.CrossFade(AttackState, 0.1f, 0, 0);
        //    }
        //}
        //---------------------------------------------------------------------
        // gravity for fall of this character
        //---------------------------------------------------------------------
        private void GRAVITY()
        {
            if (_characterController.enabled)
            {
                if (CheckGrounded())
                {
                    if (MoveDirection.y < -0.1f)
                    {
                        MoveDirection.y = -0.1f;
                    }
                }
                MoveDirection.y -= 0.1f;
                _characterController.Move(MoveDirection * Time.deltaTime);
            }
        }
        //---------------------------------------------------------------------
        // whether it is grounded
        //---------------------------------------------------------------------
        private bool CheckGrounded()
        {
            if (_characterController.isGrounded && _characterController.enabled)
            {
                return true;
            }
            Ray ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
            float range = 0.2f;
            return Physics.Raycast(ray, range);
        }
        //---------------------------------------------------------------------
        // for slime moving
        //---------------------------------------------------------------------
        private void MOVE()
        {
            // velocity
            if (Anim.GetCurrentAnimatorStateInfo(0).fullPathHash == MoveState)
            {
                if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) &&
            !(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
                {
                    MOVE_Velocity(new Vector3(0, 0, -Speed), new Vector3(0, 180, 0));
                }
                else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) &&
                 !(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)))
                {
                    MOVE_Velocity(new Vector3(0, 0, Speed), new Vector3(0, 0, 0));
                }
                else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) &&
                 !(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
                {
                    MOVE_Velocity(new Vector3(Speed, 0, 0), new Vector3(0, 90, 0));
                }
                else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) &&
                 !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
                {
                    MOVE_Velocity(new Vector3(-Speed, 0, 0), new Vector3(0, 270, 0));
                }
            }
            KEY_DOWN();
            KEY_UP();
        }
        //---------------------------------------------------------------------
        // value for moving
        //---------------------------------------------------------------------
        private void MOVE_Velocity(Vector3 velocity, Vector3 rot)
        {
            MoveDirection = new Vector3(velocity.x, MoveDirection.y, velocity.z);
            if (_characterController.enabled)
            {
                _characterController.Move(MoveDirection * Time.deltaTime);
            }
            MoveDirection.x = 0;
            MoveDirection.z = 0;
            this.transform.rotation = Quaternion.Euler(rot);
        }
        //---------------------------------------------------------------------
        // whether arrow key is key down
        //---------------------------------------------------------------------
        private void KEY_DOWN()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                Anim.CrossFade(MoveState, 0.1f, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                Anim.CrossFade(MoveState, 0.1f, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                Anim.CrossFade(MoveState, 0.1f, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                Anim.CrossFade(MoveState, 0.1f, 0, 0);
            }
        }
        //---------------------------------------------------------------------
        // whether arrow key is key up
        //---------------------------------------------------------------------
        private void KEY_UP()
        {
            // Проверка отпускания стрелок или WASD
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
            {
                if (!Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.S) &&
                    !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.A) &&
                    !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.D))
                {
                    Anim.CrossFade(IdleState, 0.1f, 0, 0);
                }
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            {
                if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.W) &&
                    !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.A) &&
                    !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.D))
                {
                    Anim.CrossFade(IdleState, 0.1f, 0, 0);
                }
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.W) &&
                    !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.S) &&
                    !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.D))
                {
                    Anim.CrossFade(IdleState, 0.1f, 0, 0);
                }
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.W) &&
                    !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.S) &&
                    !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.A))
                {
                    Anim.CrossFade(IdleState, 0.1f, 0, 0);
                }
            }
        }
        //---------------------------------------------------------------------
        // damage
        //---------------------------------------------------------------------


        public void Damage()
        {
            Anim.CrossFade(SurprisedState, 0.1f, 0, 0);
            // Если таймер неуязвимости еще тикает — игнорируем урон
            if (_invincibilityTimer > 0f) return;

            // Включаем неуязвимость на полсекунды, чтобы лава не спамила уроном
            _invincibilityTimer = 0.5f;

            // Увеличиваем счетчик смертей!
            DataContainer._deaths++;
            Debug.Log("Смертей всего: " + DataContainer._deaths);

            // Отключаем контроллер, чтобы пропустить телепорт
            if (_characterController != null) _characterController.enabled = false;

            // Телепортируем на спавн
            transform.position = _respawn.position;
            transform.rotation = _respawn.rotation;

            // Обнуляем скорость, чтобы призрак не летел по инерции после возрождения
            MoveDirection = Vector3.zero;

            // Включаем контроллер обратно
            if (_characterController != null) _characterController.enabled = true;
        }

        //---------------------------------------------------------------------
        // respawn
        //---------------------------------------------------------------------
        private void Respawn()
        {

            // player HP
            isDead = false;

            _characterController.enabled = false;
            this.transform.position = _respawn.position; // player position
            this.transform.rotation = _respawn.rotation; // player facing
            _characterController.enabled = true;

            // reset Dissolve
            Dissolve_value = 1;
            for (int i = 0; i < MeshR.Length; i++)
            {
                MeshR[i].material.SetFloat("_Dissolve", Dissolve_value);
            }
            // reset animation
            Anim.CrossFade(IdleState, 0.1f, 0, 0);
        }




        public void SetNewRespawnPoint(Transform newRespawn)
        {
            _respawn = newRespawn;
        }




        public void LockInput()
        {
            _characterController.enabled = false;
        }

        // Проверка столкновения с НПС со стороны Призрака
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // Проверяем, есть ли на объекте, в который мы врезались, скрипт EnemyAI
            if (hit.gameObject.GetComponent<EnemyAI>() != null)
            {
                Damage(); // Мгновенно убиваем призрака и отправляем на спавн
            }
        }
    }
}