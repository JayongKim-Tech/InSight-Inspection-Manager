🔍 프로젝트명: In-Sight Inspection Manager (ISIM)
📌 Project Overview
본 프로젝트는 Cognex In-Sight 스마트 카메라를 활용하여 제조 현장의 라인에서 실시간으로 제품을 검사하고, 데이터를 관리하는 C# WPF 기반 통합 제어 소프트웨어입니다. 단순한 이미지 판독을 넘어, 상위 시스템(DB) 연동 및 레시피 관리 기능을 포함한 실제 생산 설비용 아키텍처를 지향합니다.

🛠 Tech Stack
Language: C# (.NET Framework 4.8)

UI Framework: WPF (Windows Presentation Foundation)

Vision Library: Cognex In-Sight SDK

Database: SQLite (또는 MS-SQL)

Protocol: TCP/IP Native Mode, SDK Event Handling

🚀 Key Functions (핵심 기능)
Real-time Monitoring: In-Sight SDK를 활용한 검사 영상 실시간 스트리밍 및 UI 렌더링.

Smart Spreadsheet Interfacing: In-Sight 스프레드시트의 특정 셀 데이터(치수, 판정 결과)를 실시간 파싱 및 데이터 구조화.

Recipe Management: 제품 모델별 Job(.job) 파일 동적 로드 및 파라미터 원격 제어.

Data Persistence: 검사 로그(OK/NG, 치수 데이터, 타임스탬프)의 로컬 DB 저장 및 조회.

Error Handling: 통신 단절 시 자동 재접속(Auto-Reconnect) 로직 및 예외 처리.

💡 팁: 왜 이렇게 써야 하나요?
"C# WPF": 최신 장비 UI 트렌드를 반영했음을 보여줍니다.

"Real-time" / "Data Persistence": 장비 SW에서 가장 중요한 실시간성과 데이터 보존 능력을 강조합니다.

"Recipe Management": 현업 장비에서 필수적인 모델 교체(Changeover) 대응 능력을 보여줍니다.
