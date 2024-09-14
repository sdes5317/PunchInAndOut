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
