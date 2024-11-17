using System.Collections.Generic;
using BaseMe;
using UnityEngine;

public class AdjustKeysManager : Singleton<AdjustKeysManager>
{
    [SerializeField]
    private bool isEnabled = true; // Thuộc tính Enable cho phép bật/tắt quản lý sự kiện

    [SerializeField]
    private List<EventEntry> eventList = new List<EventEntry>(); // Danh sách sự kiện hiển thị trong Inspector

    private Dictionary<EventType, EventData> events = new Dictionary<EventType, EventData>(); // Dictionary phục vụ cho tìm kiếm nhanh

    Dictionary<EventType, string> defaultEventNames = new Dictionary<EventType, string>
    {
        { EventType.InterCallLoad, "adj_inter_call_load" },
        { EventType.InterLoadSuccess, "adj_inters_load_success" },
        { EventType.InterLoadFail, "adj_inters_load_fail" },
        { EventType.InterCallShow, "adj_inters_call_show" },
        { EventType.InterNotPassedCapping, "adj_inters_not_passed_capping_time" },
        { EventType.InterPassedCapping, "adj_inters_passed_capping_time" },
        { EventType.InterAvailable, "adj_inters_available" },
        { EventType.InterNotAvailable, "adj_inters_not_available" },
        { EventType.InterDisplay, "adj_inters_displayed" },
        { EventType.InterShowFail, "adj_inters_show fail" },
        { EventType.RewardCallLoad, "adj_reward_call_load" },
        { EventType.RewardLoadSuccess, "adj_reward_load_success" },
        { EventType.RewardLoadFail, "adj_rewards_load_fail" },
        { EventType.RewardCallShow, "adj_rewards_call_show" },
        { EventType.RewardAvailable, "adj_rewards_available" },
        { EventType.RewardNotAvailable, "adj_rewards_not_available" },
        { EventType.RewardDisplay, "adj_rewards_displayed" },
        { EventType.RewardShowFail, "adj_rewards_show fail" },
        { EventType.RewardCompleted, "adj_rewarded_completed" }
    };

    public void InitializeDefaultEvents()
    {
        eventList.Clear();
        foreach (var kvp in defaultEventNames)
        {
            EventEntry entry = new EventEntry
            {
                eventType = kvp.Key,
                eventData = new EventData
                {
                    eventNameKey = kvp.Value,
                    eventNameValueAndroid = "",  // Giá trị mặc định cho Android
                    eventNameValueiOS = ""       // Giá trị mặc định cho iOS
                }
            };
            eventList.Add(entry);
        }
        SyncEventsDictionary();
    }

    // Phương thức đồng bộ hóa danh sách và Dictionary
    private void SyncEventsDictionary()
    {
        events.Clear(); // Xóa các giá trị cũ

        foreach (var entry in eventList)
        {
            if (!events.ContainsKey(entry.eventType))
            {
                events.Add(entry.eventType, entry.eventData); // Thêm sự kiện mới vào Dictionary
            }
            else
            {
                events[entry.eventType] = entry.eventData; // Cập nhật sự kiện nếu đã tồn tại
            }
        }
    }

    // Hàm GetEvent chỉ trả về dữ liệu khi Manager được bật
    public EventData GetEvent(EventType eventType)
    {
        if (!isEnabled) // Nếu Manager không được bật, luôn trả về null
        {
            return null;
        }

        return events.TryGetValue(eventType, out var eventData) ? eventData : null;
    }

    public string GetToken(EventType eventType)
    {
        var eventData = GetEvent(eventType);
        if (eventData != null)
        {
#if UNITY_ANDROID
                return eventData.eventNameValueAndroid;
#elif UNITY_IOS
            return eventData.eventNameValueiOS;
#else
                return ""; // Trả về chuỗi rỗng nếu không phải Android hoặc iOS
#endif
        }

        return "";
    }

    // Class chứa dữ liệu của sự kiện
    [System.Serializable]
    public class EventData
    {
        public string eventNameKey;
        public string eventNameValueAndroid; // Giá trị cho Android
        public string eventNameValueiOS;     // Giá trị cho iOS
    }

    // Class chứa cặp EventType và EventData
    [System.Serializable]
    public class EventEntry
    {
        public EventType eventType;
        public EventData eventData;
    }

    // Enum các loại sự kiện
    public enum EventType
    {
        InterCallLoad, InterLoadSuccess, InterLoadFail, InterCallShow,
        InterPassedCapping, InterNotPassedCapping, 
        InterAvailable, InterNotAvailable, InterDisplay, InterShowFail,
        RewardCallLoad, RewardLoadSuccess, RewardCallShow, RewardAvailable, RewardNotAvailable,
        RewardDisplay, RewardCompleted, RewardLoadFail, RewardShowFail
    }
}
