using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Person
{
    public string name;
    public int age;
}

public class PersonList
{
    public Person[] Persons;
}

public class JsonUtilityTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {


        Person person = new Person();
        person.name = "Jack";
        person.age = 12;

        string jsonString = JsonUtility.ToJson(person);
        Debug.Log(jsonString);

        Person person2 = new Person();
        person2.name = "Mark";
        person2.age = 24;

        PersonList pList = new PersonList();
        Person[] p = new Person[] { person, person2 };
        pList.Persons = p;

        jsonString = JsonUtility.ToJson(pList);
        Debug.Log(jsonString);
    }


}
