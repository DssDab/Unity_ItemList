using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager
{
    public string m_Name;
    public int m_Level;
    public int m_Tier;
    public int m_Price;
    public int m_Order;
    public float m_LvPer;
    public float m_TrPer;
    public ItemManager(string name, int Level, int Tier, int Price, int order, float LvPer, float TrPer)
    {
        m_Name = name;
        m_Level = Level;
        m_Tier = Tier;
        m_Price = Price;
        m_Order = order;
        m_LvPer = LvPer;
        m_TrPer = TrPer;
    }

}
public class ItemList_Mgr : MonoBehaviour
{
    List<ItemManager> m_ItemMgr = new List<ItemManager>();
    List<ItemManager> m_ItemTemp = new List<ItemManager>();

    [Header("---ItemSearch---")]
    public InputField m_ItemName;
    public Button m_AddBtn;
    public Button m_SearchBtn;

    [Header("---ItemInfo---")]
    public Button m_RemoveBtn;
    public Button m_LvUpBtn;
    public Button m_TrUpBtn;
    public Text m_LvPercentText;
    public Text m_TierPercentText;
    public Text m_ItemInfoText;
    public Text m_UpSucText;
    public GameObject m_Info;
    [Header("---SortingGroup---")]
    public Button m_FISortBtn;
    public Button m_LvSortBtn;
    public Button m_TierSortBtn;
    public Button m_CleanAllBtn;
    public Text m_ItemListText;
    

    float TextTime = 3.0f;
    

    int FiSort(ItemManager x, ItemManager y)
    {
        return x.m_Order.CompareTo(y.m_Order);
    }

    int LvSort(ItemManager x, ItemManager y)
    {
        return x.m_Level.CompareTo(y.m_Level);
    }

    int TierSort(ItemManager x, ItemManager y)
    {
        return x.m_Tier.CompareTo(y.m_Tier);
    }

    void Start()
    {
        LoadList();
        RefreshUiList();

        if (m_AddBtn != null)
            m_AddBtn.onClick.AddListener(AddClick);
        if (m_SearchBtn != null)
            m_SearchBtn.onClick.AddListener(SearchClick);
        if (m_RemoveBtn != null)
            m_RemoveBtn.onClick.AddListener(()=> { 
                for(int i=0; i< m_ItemMgr.Count;)
                {
                    string a_Node = m_ItemMgr[i].m_Name; 

                   if(m_ItemName.text == a_Node)
                    {
                        PlayerPrefs.DeleteKey(a_Node);
                        m_ItemMgr.RemoveAt(i);
                        SaveList();
                        RefreshUiList();
                        m_ItemInfoText.text = "";
                    }
                   else
                    {
                        i++;
                    }
                }
            });
        if (m_LvUpBtn != null)
            m_LvUpBtn.onClick.AddListener(LvUpClick);
        if (m_TrUpBtn != null)
            m_TrUpBtn.onClick.AddListener(TrUpClick);
        if (m_FISortBtn != null)
            m_FISortBtn.onClick.AddListener(() =>
            {

                m_ItemMgr.Sort(FiSort);
                SaveList();
                RefreshUiList();
            });
        if (m_LvSortBtn != null)
            m_LvSortBtn.onClick.AddListener(()=>
            {
                m_ItemMgr.Sort(LvSort);
                SaveList();
                RefreshUiList();
            });
        if (m_TierSortBtn != null)
            m_TierSortBtn.onClick.AddListener(()=>
            {
                m_ItemMgr.Sort(TierSort);
                SaveList();
                RefreshUiList();
            });
        if (m_CleanAllBtn != null)
            m_CleanAllBtn.onClick.AddListener(()=>
            {
                PlayerPrefs.DeleteAll();
                m_ItemMgr.Clear();
                m_ItemName.text = "";
                m_Info.gameObject.SetActive(false);
                RefreshUiList();
            });

    }
    void Update()
    {
        TextTime -= Time.deltaTime;
        if(TextTime<=0)
        {
            TextTime = 0.0f;
            m_UpSucText.text = "";
            if(string.IsNullOrEmpty(m_ItemName.text) == true)
            {
                m_ItemInfoText.text = "";
            }
        }
    }

    private void AddClick()
    {
        string a_Itemname = m_ItemName.text;
        m_ItemName.text = "";
        a_Itemname = a_Itemname.Trim();
        int a_Level = Random.Range(1,9);
        int a_Tier = Random.Range(1, 8);
        int a_Price = Random.Range(700, 1601);
        int a_Order = 0;
        float a_LvPer = 100.0f;
        float a_TLvPer = 100.0f;


        if (a_Itemname.Length<=0) 
           return;
        

        for(int i =0; i< m_ItemMgr.Count;i++)
        {
            if (a_Itemname == m_ItemMgr[i].m_Name)
                return;

            a_Order++;
        }
        ItemManager a_Node = new ItemManager(a_Itemname, a_Level, a_Tier, a_Price, a_Order, a_LvPer, a_TLvPer);
        m_ItemMgr.Add(a_Node);
        
        SaveList();
        RefreshUiList();
        

    }
    private void SearchClick()
    {
        string SameText = m_ItemName.text;
        float LvPer = 100.0f;
        float TrPer = 100.0f;

        if (string.IsNullOrEmpty(SameText) == true)
            return;
        ItemManager a_Node = m_ItemMgr.Find(Obj=> Obj.m_Name == SameText);
        if (a_Node == null)
        {
            m_UpSucText.text = "ã�� �������� �������� �ʽ��ϴ�";
            TextTime = 3.0f;
            return;
        }
        m_ItemInfoText.text = string.Format("\"[{0}] : ����({1}) ���({2}) ����({3}) \n",
        a_Node.m_Name, a_Node.m_Level, a_Node.m_Tier, a_Node.m_Price);
        for (int i = 1; i < a_Node.m_Level; i++)
        {
            LvPer *= 0.96f;
        }
        a_Node.m_LvPer = LvPer;
        for (int j = 1; j < a_Node.m_Tier; j++)
        {
            TrPer *= 0.96f;
        }
        a_Node.m_TrPer = TrPer;
        for (int i=0;i<m_ItemMgr.Count;i++) 
        {
            m_ItemMgr[i].m_LvPer = (int)a_Node.m_LvPer;
            m_ItemMgr[i].m_TrPer = (int)a_Node.m_TrPer;
        }
        
        
        SaveList();
        RefreshUiList();
        m_Info.gameObject.SetActive(true);
    }
    private void LvUpClick()
    {
        int LvUp = Random.Range(1, 101);
        string TempName = m_ItemName.text;
        if (string.IsNullOrEmpty(m_ItemInfoText.text) == true)
            return;
        for(int i =0; i < m_ItemMgr.Count;i++)
        {

           if (TempName == m_ItemMgr[i].m_Name)
           {

                    if (m_ItemMgr[i].m_Level >= 1 && LvUp <= m_ItemMgr[i].m_LvPer)
                    {
                            m_ItemMgr[i].m_LvPer *= 0.96f;
                            m_ItemMgr[i].m_Level++;
                            m_UpSucText.text = "��ȭ�� �����ϼ̽��ϴ�!!";
                    m_ItemInfoText.text = string.Format("\"[{0}] : ����({1}) ���({2}) ����({3}) \n",
                                                m_ItemMgr[i].m_Name, m_ItemMgr[i].m_Level, m_ItemMgr[i].m_Tier, m_ItemMgr[i].m_Price);
                    m_LvPercentText.text = "Ȯ�� : " + (int)m_ItemMgr[i].m_LvPer + "%";
                    TextTime = 3.0f;
                            return;
                    }
                    else
                    {
                    PlayerPrefs.DeleteKey(m_ItemMgr[i].m_Name);
                    m_ItemMgr.RemoveAt(i);
                    m_UpSucText.text = "��ȭ�� �����Ͽ� �������� �ı� �Ǿ����ϴ�...";
                    TextTime = 3.0f;
                    m_ItemInfoText.text = "";
                    }
                    

           }
           
        }
        SaveList();
        RefreshUiList();
    }       
    private void TrUpClick()
    {

    }
 
    void SaveList()
    {
        PlayerPrefs.DeleteAll();

        if (m_ItemMgr.Count <= 0)
            return;

        PlayerPrefs.SetInt("ItemCount", m_ItemMgr.Count);
        ItemManager a_Node;
        string a_KeyBuff = "";

        for(int i=0;i<m_ItemMgr.Count;i++)
        {
            a_Node = m_ItemMgr[i];
            a_KeyBuff = string.Format("ItemList_Name{0}", i);
            PlayerPrefs.SetString(a_KeyBuff, a_Node.m_Name);
            a_KeyBuff = string.Format("ItemList_Level{0}", i);
            PlayerPrefs.SetInt(a_KeyBuff, a_Node.m_Level);
            a_KeyBuff = string.Format("ItemList_Tier{0}", i);
            PlayerPrefs.SetInt(a_KeyBuff, a_Node.m_Tier);
            a_KeyBuff = string.Format("ItemList_Price{0}", i);
            PlayerPrefs.SetInt(a_KeyBuff, a_Node.m_Price);
            a_KeyBuff = string.Format("ItemList_Order{0}", i);
            PlayerPrefs.SetInt(a_KeyBuff, a_Node.m_Order);
            a_KeyBuff = string.Format("Item_LvPer{0}", i);
            PlayerPrefs.SetFloat(a_KeyBuff, a_Node.m_LvPer);
            a_KeyBuff = string.Format("Item_TrPer{0}", i);
            PlayerPrefs.SetFloat(a_KeyBuff, a_Node.m_TrPer);
        }


    }
    void LoadList()
    {
        int a_ItemCount = PlayerPrefs.GetInt("ItemCount", 0);

        if (a_ItemCount <= 0)
            return;
        ItemManager a_Node;
        string a_KeyBuff;

        for(int i =0;i<a_ItemCount;i++)
        {
            a_KeyBuff = string.Format("ItemList_Name{0}", i);
            string a_ItemName = PlayerPrefs.GetString(a_KeyBuff, "");
            a_KeyBuff = string.Format("ItemList_Level{0}", i);
            int a_ItemLevel = PlayerPrefs.GetInt(a_KeyBuff, 0);
            a_KeyBuff = string.Format("ItemList_Tier{0}", i);
            int a_ItemTier = PlayerPrefs.GetInt(a_KeyBuff, 0);
            a_KeyBuff = string.Format("ItemList_Price{0}", i);
            int a_ItemPrice = PlayerPrefs.GetInt(a_KeyBuff, 0);
            a_KeyBuff = string.Format("ItemList_Order{0}", i);
            int a_Order = PlayerPrefs.GetInt(a_KeyBuff, 0);
            a_KeyBuff = string.Format("Item_LvPer{0}", i);
            float a_LvPer = PlayerPrefs.GetInt(a_KeyBuff, 0);
            a_KeyBuff = string.Format("Item_TrPer{0}", i);
            float a_TrPer = PlayerPrefs.GetInt(a_KeyBuff, 0);
            a_Node = new ItemManager(a_ItemName, a_ItemLevel, a_ItemTier, a_ItemPrice, a_Order, a_LvPer, a_TrPer);
            m_ItemMgr.Add(a_Node);
        }
    }
    void RefreshUiList()
    {
        string a_KeyBuff = "";
        string a_LvPer = "";
        string a_TrPer = "";
        for (int i = 0; i < m_ItemMgr.Count; i++)
        {
            a_KeyBuff += string.Format("[{0}] : ����({1}) ���({2}) ����({3}) ",
                                        m_ItemMgr[i].m_Name, m_ItemMgr[i].m_Level, m_ItemMgr[i].m_Tier, m_ItemMgr[i].m_Price);
            a_KeyBuff += "\n";

            a_LvPer = string.Format("Ȯ�� : {0}%", m_ItemMgr[i].m_LvPer);
            a_TrPer = string.Format("Ȯ�� : {0}%", m_ItemMgr[i].m_TrPer);

        }
        if (m_ItemListText != null)
            m_ItemListText.text = a_KeyBuff;
            if(m_LvPercentText != null)
                m_LvPercentText.text = a_LvPer;
            if(m_TierPercentText != null)
                m_TierPercentText.text = a_TrPer;

            
    }
}
