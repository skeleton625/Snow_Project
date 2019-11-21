using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Blocks;
    [SerializeField]
    private int RowBlockSize;
    [SerializeField]
    private int ColBlockSize;
    [SerializeField]
    private Vector3 BlockStartPos;

    // Start is called before the first frame update
    void Start()
    {
        GenPlayField();   
    }

    private void GenPlayField()
    {
        int _types = 0;
        for(int i = 0; i < RowBlockSize; i++)
        {
            for(int j = 0; j < ColBlockSize; j++)
            {
                Vector3 _pos = BlockStartPos + new Vector3(2 * i, 0, 2 * j);
                GameObject _clone = Instantiate(Blocks[_types%2], _pos, Quaternion.identity);
                _clone.name = "Grounds";
                _clone.transform.parent = gameObject.transform;
                ++_types;
            }
        }
    }
}
