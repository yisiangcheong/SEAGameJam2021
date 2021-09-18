using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PinState
{
    Normal,
    Attack,
    Dead,
}

public class PinController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Rigidbody movementBody = null;
    [SerializeField] BoxCollider movementColl = null;
    [SerializeField] Rigidbody attackBody = null;
    [SerializeField] CapsuleCollider attackColl = null;
    [SerializeField] Rigidbody meshBody = null;
    [SerializeField] MeshCollider meshColl = null;
    [SerializeField] MeshRenderer[] meshRenderers = null;
    [SerializeField] PinAttackCollider pinAttackCollider = null;
    [SerializeField] PinModelCollider pinModelCollider = null;

    [Header("Settings")]
    [SerializeField] float cascadeMultiplier = 1.5f;
    [SerializeField] float cascadeReduction = 0.01f;
    public float CascadeMultiplier { get { return cascadeMultiplier; } }

    [SerializeField] float deactivationDelay = 10.0f;
    [SerializeField] float deactivationDuration = 1.0f;
    [SerializeField] float turnSpeed = 2.0f;
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float minHuntRange = 35.0f;
    [SerializeField] float attackRange = 5.0f;
    [SerializeField] float attackDelay = 0.5f;
    [SerializeField] float leanForce = 2.0f;
    [SerializeField] float forwardJumpForce = 5.0f;
    [SerializeField] float upwardJumpForce = 10.0f;

    [SerializeField] float minimumForceToDie = 10.0f;

    //if TotalHitCount % bowlingStrikeAmount == 0, play the strike SFX
    [SerializeField] int bowlingStrikeAmount = 10;

    [Header("Preview Controls")]
    [SerializeField] bool selfInit = false;
    [SerializeField] float force = 10.0f;
    [SerializeField] bool explodeForward = false;
    [SerializeField] bool isBraindead = false;

    [Header("Preview")]
    [SerializeField] Transform target = null;
    
    public PinState pinstate { get; private set; } = PinState.Normal;

    Coroutine behaviourRoutine = null;
    Coroutine deactivateRoutine = null;

    WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

    HitEffectPooler hitEffectPooler = null;
    MultiplierMenu multiplierMenu = null;
    PinSpawnManager spawnManager = null;

    private void OnEnable()
    {
        if (selfInit) Invoke("Initialize", 2.0f);
    }

    private void Initialize()
    {
        pinstate = PinState.Normal;

        movementColl.enabled = true;
        meshColl.enabled = false;

        if (explodeForward)
        {
            Die(force, new Vector3(0.0f, 0.0f, 1.0f), true);
        }

        if (!isBraindead)
        {
            FindTarget();
        }
    }

    public void Initialize(PinSpawnManager spawnManager, MultiplierMenu multiplierMenu, HitEffectPooler hitEffectPooler)
    {
        this.spawnManager = spawnManager;
        this.multiplierMenu = multiplierMenu;
        this.hitEffectPooler = hitEffectPooler;
        Initialize();
    }

    public void Die(float launchForce, Vector3 direction, bool addToScore, bool instantDecoration = false)
    {
        pinstate = PinState.Dead;
        StopAllCoroutines();

        if (addToScore)
        {
            multiplierMenu.AddScore();

            if(multiplierMenu.totalHitCount%bowlingStrikeAmount == 0)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.PinBowlingStrikeEvent, gameObject);
            }
        }
        if (instantDecoration) pinModelCollider.TurnIntoDecoration();

        movementBody.isKinematic = true;
        movementBody.useGravity = false;
        movementBody.velocity = Vector3.zero;

        attackBody.isKinematic = true;
        attackBody.useGravity = false;
        attackBody.velocity = Vector3.zero;

        movementColl.enabled = false;
        attackColl.enabled = false;
        meshColl.enabled = true;

        meshBody.velocity = Vector3.zero;
        meshBody.isKinematic = false;
        meshBody.useGravity = true;

        meshBody.AddTorque(direction * launchForce, ForceMode.Impulse);
        meshBody.AddForce(direction * launchForce, ForceMode.Impulse);

        if (deactivateRoutine != null) StopCoroutine(deactivateRoutine);
        deactivateRoutine = StartCoroutine(DeactivateRoutine());

        if (spawnManager != null) spawnManager.RemovePin(gameObject);
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    void FindTarget()
    {
        if (target == null && GameObject.FindGameObjectWithTag("Player") != null) target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target != null)
        {
            if (behaviourRoutine != null) StopCoroutine(behaviourRoutine);
            behaviourRoutine = StartCoroutine(MovementRoutine(target));
        }
    }

    IEnumerator MovementRoutine(Transform target)
    {
        Vector3 targetDirection = Vector3.zero;
        Vector3 newDirection = Vector3.zero;

        while (true)
        {
            if (Vector3.Distance(target.position, transform.position) >= minHuntRange)
            {
                yield return null;
                continue;
            }

            targetDirection = target.position - transform.position;
            newDirection = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);

            transform.position = transform.position + (transform.forward * (moveSpeed * Time.deltaTime));

            if (Vector3.Distance(transform.position, target.position) <= attackRange) break;

            yield return fixedUpdate;
        }

        float timer = 0.0f;

        while (timer < attackDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        BeginAttack();
    }

    void BeginAttack()
    {
        movementBody.isKinematic = true;
        movementBody.useGravity = false;
        movementBody.velocity = Vector3.zero;

        movementColl.enabled = false;
        attackColl.enabled = true;

        attackBody.velocity = Vector3.zero;
        attackBody.isKinematic = false;
        attackBody.useGravity = true;

        attackBody.AddTorque(transform.right * leanForce, ForceMode.Impulse);
        attackBody.AddForce(transform.forward * forwardJumpForce + transform.up * upwardJumpForce, ForceMode.Impulse);

        pinAttackCollider.StartAttackDeactivation();

        AudioManager.Instance.PlaySFX(AudioManager.Instance.PinAttackEvent, gameObject);
    }

    void SetMaterialRenderersToFade()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            meshRenderers[i].material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            meshRenderers[i].material.SetInt("_ZWrite", 0);
            meshRenderers[i].material.DisableKeyword("_ALPHATEST_ON");
            meshRenderers[i].material.EnableKeyword("_ALPHABLEND_ON");
            meshRenderers[i].material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            meshRenderers[i].material.renderQueue = 3000;
        }
    }

    IEnumerator DeactivateRoutine()
    {
        float timer = 0.0f;

        while (timer < deactivationDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        SetMaterialRenderersToFade();

        while (timer < deactivationDuration)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material.color = new Color(meshRenderers[i].material.color.r, 
                                                            meshRenderers[i].material.color.g, 
                                                            meshRenderers[i].material.color.b, 
                                                            1.0f - (timer / deactivationDuration));
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Deactivate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("PinHazard") && pinstate != PinState.Dead)
        {
            if (collision.transform.GetComponent<Rigidbody>() != null)
            {
                Vector3 direction = collision.transform.parent.position - transform.position;
                direction = -direction.normalized;

                if (collision.transform.GetComponentInParent<PinController>() != null)
                    cascadeMultiplier = collision.transform.GetComponentInParent<PinController>().CascadeMultiplier - cascadeReduction;

                hitEffectPooler.SpawnHitEffect(transform.position);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.PinHurtEvent, gameObject);
                Die(collision.transform.GetComponent<Rigidbody>().velocity.magnitude * cascadeMultiplier, direction, true);

                Debug.Log("PunchForce: " + collision.relativeVelocity.magnitude);

                //if(collision.relativeVelocity.magnitude)
            }
        }
        else if (collision.transform.GetComponent< PlayerHandCollider>() != null && pinstate != PinState.Dead)
        {
            Vector3 direction = collision.transform.parent.position - transform.position;
            direction = -direction.normalized;

            hitEffectPooler.SpawnHitEffect(transform.position);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.PinHurtEvent, gameObject);
            Die(collision.transform.GetComponent<Rigidbody>().velocity.magnitude * (MultiplierMenu.currentMultiplierPower / 10.0f + 1.0f), direction, true);
        }
    }
}
