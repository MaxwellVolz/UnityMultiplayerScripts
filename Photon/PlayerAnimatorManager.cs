using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace Com.Narfa.MultiplayerTest
{
    public class PlayerAnimatorManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        public GameObject frisbeeInHand;

        #endregion

        #region Private Fields

        [SerializeField]
        private float directionDampTime = .25f;
        private Animator animator;

        private GameObject pickableFrisbee;



        #endregion

        #region RPCs

        [PunRPC]
        void ChatMessage(string a, string b)
        {
            Debug.Log(string.Format("ChatMessage {0} {1}", a, b));
        }

        [PunRPC]
        void FrisbeePickupSync(PhotonMessageInfo info)
        {
            Debug.Log(info);
            Debug.Log(info.Sender);

            // need to target player that sent message and update activeState of <frisbeeInHand>
            // see <PlayerUI.cs> for example?
            //frisbeeInHand.SetActive(true);
        }

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true || !animator)
            {
                return;
            }

            // deal with Jumping
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // only allow jumping if we are running.
            if (stateInfo.IsName("Base Layer.Run"))
            {
                // When using trigger parameter
                if (Input.GetButtonDown("Fire2"))
                {
                    animator.SetTrigger("Jump");
                }

                if(Input.GetKeyDown(KeyCode.Q))
                {
                    animator.SetTrigger("CatchLeft");
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    animator.SetTrigger("CatchRight");
                }
            }

            if (Input.GetKeyDown(KeyCode.F)) 
            {
                if(animator.GetBool("OnFrisbee") == true)
                {
                    animator.SetTrigger("PickUp");

                    if(pickableFrisbee != null)
                    {
                        PhotonNetwork.Destroy(pickableFrisbee);
                        this.photonView.RPC("ChatMessage", RpcTarget.All, "jup", "and jup!");
                        this.photonView.RPC("FrisbeePickupSync", RpcTarget.All);
                        frisbeeInHand.SetActive(true);
                    }
                }
            }



            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (v < 0)
            {
                v = 0;
            }
            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }

        void OnCollisionEnter(Collision col)
        {
            
        }


        void OnTriggerEnter(Collider col)
        {

            // REMOVE COMMENT IF COMPILING FOR MULTIPLAYER
            if (!photonView.IsMine)
               return;
            

            if (col.gameObject.name == "VendingMachine")
            {
                Debug.Log("Vending Machine activated.");
                //PhotonNetwork.Instantiate("FrisbeeFrefab", new Vector3(2.607f, 0.473f, 3.343f), Quaternion.identity, 0);
                //PhotonNetwork.Instantiate("FrisbeeFrefab", new Vector3(col.gameObject.transform.position.x + 2.607f, col.gameObject.transform.position.y + 0.473f, col.gameObject.transform.position.z + 3.343f), Quaternion.identity, 0);
                //PhotonNetwork.Instantiate("FrisbeePrefab", new Vector3(col.gameObject.transform.position.x + -19.7f, col.gameObject.transform.position.y+ -8.3f, col.gameObject.transform.position.z - 16.3f), Quaternion.identity, 0);
                PhotonNetwork.Instantiate("FrisbeePrefab", new Vector3(col.gameObject.transform.position.x + 0.5f, col.gameObject.transform.position.y + 0.5f, col.gameObject.transform.position.z), Quaternion.identity, 0);

            }

            if (col.gameObject.tag == "Frisbee")
            {
                pickableFrisbee = col.gameObject;
                animator.SetBool("OnFrisbee", true);
            }
            
        }

        void OnTriggerStay(Collider col)
        {
            // REMOVE COMMENT IF COMPILING FOR MULTIPLAYER
            if (!photonView.IsMine)
               return;
            if (col.gameObject.tag == "Frisbee")
            {
                pickableFrisbee = col.gameObject;
                animator.SetBool("OnFrisbee", true);
            }
        }

        void OnTriggerExit(Collider col)
        {
            // REMOVE COMMENT IF COMPILING FOR MULTIPLAYER
            if (!photonView.IsMine)
               return;

            if (col.gameObject.tag == "Frisbee")
            {
                pickableFrisbee = null;
                animator.SetBool("OnFrisbee", false);
            }
        }


        #endregion
    }
}