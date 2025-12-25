# 🔍 In-Sight Inspection Manager (ISIM)

> **Cognex In-Sight 스마트 카메라 기반의 C# WPF 통합 검사 제어 솔루션** > 본 프로젝트는 제조 현장의 실시간 제품 검사, 데이터 관리 및 상위 시스템 연동을 위한 실제 생산 설비용 아키텍처를 지향합니다.

---

### 🛠 Tech Stack

| 분류 | 기술 스택 |
| :--- | :--- |
| **Language** | C# (.NET Framework 4.8) |
| **UI Framework** | WPF (Windows Presentation Foundation) |
| **Vision Library** | Cognex In-Sight SDK |
| **Database** | SQLite (또는 MS-SQL) |
| **Protocol** | TCP/IP Native Mode, SDK Event Handling |

---

### 🚀 Key Functions (핵심 기능)

* **Real-time Monitoring**
  * In-Sight SDK를 활용한 검사 영상의 실시간 스트리밍 및 UI 렌더링
* **Smart Spreadsheet Interfacing**
  * In-Sight 스프레드시트의 특정 셀 데이터(치수, 판정 결과 등) 실시간 파싱 및 데이터 구조화
* **Recipe Management**
  * 제품 모델별 Job(`.job`) 파일 동적 로드 및 원격 파라미터 제어 기능
* **Data Persistence**
  * 검사 로그(OK/NG, 치수, 타임스탬프)의 로컬 DB 저장 및 이력 조회 시스템
* **Error Handling**
  * 통신 단절 시 자동 재접속(Auto-Reconnect) 로직 및 예외 처리 프로세스

---

### 💡 Project Insight

* **C# WPF 사용**: 최신 산업용 장비의 UI/UX 트렌드를 반영하여 사용자 편의성 증대
* **Real-time & Persistence**: 장비 SW의 핵심인 실시간 데이터 처리와 데이터 무결성 강조
* **Recipe Management**: 현업 장비에서 필수적인 모델 교체(Changeover) 대응 능력 구현
