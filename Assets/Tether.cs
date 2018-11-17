using UnityEngine;

using tetherType = Constants.TetherMode;
using stateType = Constants.TetherState;

public class Tether : MonoBehaviour {

    [SerializeField] float m_maximumLength = 5f;
    [SerializeField] float m_minimumLength = 1f;
    [SerializeField] float m_pullForce = 10f;
    [SerializeField] float m_attachAnimationDuration = 3f;
    [SerializeField] GameObject m_tetherBase;
    [SerializeField] GameObject m_tetherEnd;

    private GameObject m_startObject;
    private GameObject m_attachedObject;
    private GameObject m_tether = null;
    private GameObject m_tetherHook = null;

    private Vector3 m_tetherDestination;
    
    private float m_tetherCurrentLength = 0f;
    private float m_tetherAnimationStart = 0f; //in seconds
    
    private tetherType m_tetherMode = tetherType.Invalid;
    private stateType m_tetherState = stateType.Idle;

    private void Awake()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_tether)
        {
            return;
        }

        UpdatePosition();
        UpdatePhysics();
    }

    public void TryInitialize(GameObject startObj, Vector3 destination, tetherType tetherM)
    {
        if (m_tether != null || startObj == null)
        {
            return;
        }
        m_tether = Instantiate(m_tetherBase);
        m_tetherHook = Instantiate(m_tetherEnd);
        m_tetherHook.GetComponent<HookBehaviour>().SetTetherInstance(this);
        m_tetherMode = tetherM;
        ChangeState(stateType.Launched);
        m_startObject = startObj;
        m_tetherDestination = destination;
        m_tetherAnimationStart = Time.time;
    }
   
    public void DestroyTether()
    {
        DestroyImmediate(m_tether);
        DestroyImmediate(m_tetherHook);
        m_tether = null;
        m_startObject = null;
        m_attachedObject = null;
        m_tetherMode = tetherType.Invalid;
        ChangeState(stateType.Idle);
    }

    public void TryRetractTether(float retractTime)
    {
        if(m_tetherState == stateType.Retracting || m_startObject == null || m_tetherHook == null)
        {
            return;
        }

        float distanceToTarget = Vector3.Distance(m_startObject.transform.position, m_tetherDestination);
        if(m_tetherCurrentLength < distanceToTarget )
        {
            m_tetherDestination = m_tetherHook.transform.position;
        }

        m_tetherState = stateType.Retracting;
        m_tetherAnimationStart = retractTime;
    }   

    private void UpdatePosition()
    {
        Vector3 startPos = m_startObject.transform.position;
        Vector3 endPosition = Vector3.zero;
        
        float animationTime = (Time.time - m_tetherAnimationStart) / m_attachAnimationDuration;
        switch (m_tetherState)
        {
            case stateType.Launched:                
                endPosition = Vector3.Lerp(startPos, m_tetherDestination, animationTime);
                if (animationTime >= 1f)
                {
                    TryRetractTether(Time.time);
                }
                break;
            case stateType.Retracting:
                endPosition = Vector3.Lerp(startPos, m_tetherDestination, 1f - animationTime);
                break;
            case stateType.Attached:
                endPosition = m_attachedObject.transform.position;
                break;
            default:
                break;
        }        

        AdjustVerticalHeight(ref endPosition);
        m_tetherCurrentLength = Vector3.Distance(startPos, endPosition) / 2;        
        m_tether.transform.localScale = new Vector3(m_tether.transform.localScale.x, m_tetherCurrentLength, m_tether.transform.localScale.z);
        m_tether.transform.position = (endPosition - startPos) / 2.0f + startPos;

        m_tether.transform.rotation = Quaternion.FromToRotation(Vector3.up, endPosition - startPos);
        m_tetherHook.transform.position = endPosition;

        bool animationFinished = Time.time > (m_tetherAnimationStart + m_attachAnimationDuration);

        float minimumLength = m_minimumLength;
        if(m_tetherState == stateType.Attached)
        {
            //For the purpose of this trial, we will asume all objects are boxes and this calculation will do the trick.
            //This is not OPTIMAL solution!
            minimumLength += m_startObject.transform.parent.localScale.x / 2f + m_attachedObject.transform.localScale.x / 2f;
        }
        if ((m_tetherCurrentLength < minimumLength) && (animationFinished || m_tetherState == stateType.Retracting))
        {
            DestroyTether();
        }
    }

    private void AdjustVerticalHeight(ref Vector3 position)
    {
        position.y = m_startObject.transform.position.y;
    }

    public void ChangeState(stateType stateType)
    {
        m_tetherState = stateType;
    }

    private void UpdatePhysics()
    {
        if(m_tetherState != stateType.Attached)
        {
            return;
        }

        UpdatePullObject();
        UpdatePullToObject();
    }

    private void UpdatePullObject()
    {
        if (m_tetherMode != tetherType.PullObjectToPlayer)
        {
            return;
        }

        Vector3 originPos = m_startObject.transform.parent.position;
        Vector3 m_pullForceV = (originPos - m_attachedObject.transform.position) * m_pullForce / m_tetherCurrentLength * Time.fixedDeltaTime;
        m_attachedObject.GetComponent<Rigidbody>().velocity = m_pullForceV;
        
    }
    private void UpdatePullToObject()
    {
        if (m_tetherMode != tetherType.PullPlayerToObject)
        {
            return;
        }

        Transform origin = m_startObject.transform.parent;
        Vector3 m_pullForceV = (m_attachedObject.transform.position - origin.position) * m_pullForce / m_tetherCurrentLength * Time.fixedDeltaTime;
        origin.GetComponent<Rigidbody>().velocity = m_pullForceV;
    }

    private void OnCollisionEnter(Collision collision)
    {
       /* ContactPoint[] collidersHit = collision.contacts;
        foreach(ContactPoint contact in collidersHit)
        {
            Debug.Log("colliding with ~ " + contact.thisCollider);
        }*/
    }

    public bool Initialize(GameObject startObj, GameObject endObj, GameObject tetherBase, tetherType tetherM)
    {
        if (startObj == null || endObj == null)
        {
            return false;
        }
        Vector3 startPos = startObj.transform.position;
        Vector3 endPos = endObj.transform.position;
        float length = (endPos - startPos).magnitude / 2;
        if (length > m_maximumLength)
        {
            return false;
        }

        m_startObject = startObj;
        m_attachedObject = endObj;
        m_tether = Instantiate(tetherBase);
        m_tetherMode = tetherM;
        return true;
    }
    
    public void OnCollision(Collider collider)
    {
        if(collider == m_startObject.transform.parent.GetComponent<BoxCollider>())
        {
            return;
        }
        if(m_tetherState != stateType.Attached)
        {
            Debug.Log("Tether Attached to" + collider);
            m_tetherState = stateType.Attached;
            m_attachedObject = collider.gameObject;
        }
    }

}
