using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;

    bool _moveToDest = false;
    Vector3 _destPos;

    void Start()
    {
        Managers.Input.KeyAction -= OnKeyboard;
        Managers.Input.KeyAction += OnKeyboard;
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
    }

    void Update()
    { 
        if (_moveToDest)
        {
            Vector3 dir = _destPos - transform.position;
            if (dir.magnitude < 0.0001f)    // ���� - ���ʹ� ��Ȯ�ϰ� 0�� �ȳ����� ��찡 ����(��������)
            {
                _moveToDest = false;
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

        }

        if (_moveToDest)
        {
            Animator anim = GetComponent<Animator>();
            anim.Play("RUN");
        }
        else
        {
            Animator anim = GetComponent<Animator>();
            anim.Play("WAIT");
        }
    }

    void OnKeyboard()
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
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))
        {
            _destPos = hit.point;
            _moveToDest = true;
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