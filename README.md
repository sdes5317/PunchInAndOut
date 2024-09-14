# PunchInAndOut
想要一個Work Do自動打卡系統，並且有以下功能
	1. 可以部屬在Linux Docker上
	2. 預設周一到周五自動打卡
	3. 可以忽略休假時間(實作邏輯預計參考同事大大的程式碼)
	4. 成功或失敗會Line Notify通知

## PunchInAndOut：
- Scheduler：
- Web api：提供有 UI 介面的排程功能，用來設定打卡時間。
  - 直接打卡功能。
  - 查詢假期功能。

## LineNotifyApi：
- 通知系統，負責將打卡成功或失敗的通知推送給用戶。

### Development Guide
1. Line UserToken設定:  
這個Token是用來讓服務能發送訊息至使用者帳號，權杖發行的方式參考[line 發行存取權杖（開發人員用）]。
在本地開發環境下,推薦使用secrets.json方式儲存,快速設定的指令如下:
```.NET CLI
dotnet user-secrets set "Line:UserToken" "YOUR_LINE"
```
