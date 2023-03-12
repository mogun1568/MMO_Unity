using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;

    Vector3 _destPos;

    void Start()
    {
        //Managers.Input.KeyAction -= OnKeyboard;
        //Managers.Input.KeyAction += OnKeyboard;
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
    }

    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
    }

    PlayerState _state = PlayerState.Idle;

    void UpdateDie()
    {
        // �ƹ��͵� ����
    }

    void UpdateMoving()
    {
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.0001f)    // ���� - ���ʹ� ��Ȯ�ϰ� 0�� �ȳ����� ��찡 ����(��������)
        {
            _state = PlayerState.Idle;
        }
        else
        {
            // Mathf.Clamp(value, min, max) -> value�� min���� �۰ų� max���� ũ�� min, max�� ��ȯ
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
            // .normalized -> ũ�Ⱑ 1�� �������ͷ� ��ȯ
            transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

            // ȸ���� �ε巴�� ���� ���� rotation���� ��ü
            //transform.LookAt(_destPos);
        }

        // �ִϸ��̼�
        Animator anim = GetComponent<Animator>();
        // ���� ���� ���¿� ���� ������ �Ѱ��ش�
        anim.SetFloat("speed", _speed);


        /* ���� �ƴ� �ڵ�� �ִϸ��̼��� �����ϴ� ���
        wait_run_ratio = Mathf.Lerp(wait_run_ratio, 1, 10.0f * Time.deltaTime);
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("wait_run_ratio", wait_run_ratio);
        anim.Play("WAIT_RUN");*/
    }

    /*  Animation Event �ǽ�
    void OnRunEvent(int a)
    {
        Debug.Log($"�ѹ� �ѹ�~~ {a}");
    }

    // �� ���� �� ���� �ִ� ���� �����Ѵ� (���⼭�� int ������ ����)
    void OnRunEvent(string a)
    {
        Debug.Log($"�ѹ� �ѹ�~~ {a}");
    }*/

    void UpdateIdle()
    {
        // �ִϸ��̼�
        Animator anim = GetComponent<Animator>();

        anim.SetFloat("speed", 0);


        /* ���� �ƴ� �ڵ�� �ִϸ��̼��� �����ϴ� ���
        wait_run_ratio = Mathf.Lerp(wait_run_ratio, 0, 10.0f * Time.deltaTime);
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("wait_run_ratio", wait_run_ratio);
        anim.Play("WAIT_RUN");*/
    }

    void Update()
    { 
        switch (_state)
        {
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
        }
    }

    /*void OnKeyboard()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            transform.position += Vector3.forward * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
            transform.position += Vector3.back * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * _speed;
        }

        _moveToDest = false;
    }*/

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))
        {
            _destPos = hit.point;
            _state = PlayerState.Moving;
            //Debug.Log($"Raycast Camera @ {hit.collider.gameObject.tag}");
        }
    }
}


// GameObject (Player)
// Transform
// PlayerController (*)

// Local -> World
// TransformDirection

// World -> Local
// InverseTransformDirection

/* position, rotation �ǽ�
float _yAngle = 0.0f;
void Update()
{
    _yAngle += Time.deltaTime * 100.0f;

    // +- delta
    //transform.Rotate(new Vector3(0.0f, Time.deltaTime * 100.0f, 0.0f));

    //transform.rotation = Quaternion.Euler(new Vector3(0.0f, _yAngle, 0.0f));

    if (Input.GetKey(KeyCode.W))
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
        transform.position += Vector3.forward * Time.deltaTime * _speed;
        //transform.Translate(Vector3.forward * Time.deltaTime * _speed);
        //transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed);
    }

    if (Input.GetKey(KeyCode.S))
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
        transform.position += Vector3.back * Time.deltaTime * _speed;
        //transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    }

    if (Input.GetKey(KeyCode.A))
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
        transform.position += Vector3.left * Time.deltaTime * _speed;
        //transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    }

    if (Input.GetKey(KeyCode.D))
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
        transform.position += Vector3.right * Time.deltaTime * _speed;
        //transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    }

}*/

/* vector ���� �ǽ�
// 1. ��ġ ����
// 2. ���� ����
struct MyVector
{
    public float x;
    public float y;
    public float z;

    //          +
    //     +    +
    // +--------+
    public float magnitude { get { return Mathf.Sqrt(x * x + y * y + z * z); } }
    public MyVector normalized { get { return new MyVector(x / magnitude, y / magnitude, z / magnitude); } }

    public MyVector(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

    public static MyVector operator +(MyVector a, MyVector b)
    {
        return new MyVector(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static MyVector operator -(MyVector a, MyVector b)
    {
        return new MyVector(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static MyVector operator *(MyVector a, float d)
    {
        return new MyVector(a.x * d, a.y * d, a.z * d);
    }
}

void Start()
{
    MyVector to = new MyVector(10.0f, 0.0f, 0.0f);
    MyVector from = new MyVector(5.0f, 0.0f, 0.0f);
    MyVector dir = to - from; // (5.0f, 0.0f, 0.0f)

    dir = dir.normalized; // (1.0f, 0.0f, 0.0f)

    MyVector newPos = from + dir * _speed;

    // ���� ����
    // 1. �Ÿ�(ũ��)    5  magnitude
    // 2. ���� ����     ->
}*/