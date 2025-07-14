using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace ChatOrg.Chat
{
    public class ExcuteAPI
    {
        private static readonly IConfiguration? Configuration;
        private static string _antapiKey = string.Empty;
        private static string _openAIapiKey = string.Empty;
        private static readonly string apiUrl = "https://api.anthropic.com/v1/messages";
        private static readonly string apiUrl1 = "https://api.openai.com/v1/responses";

        static ExcuteAPI()
        {
            try
            {
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                if (string.IsNullOrEmpty(basePath))
                {
                    throw new InvalidOperationException("애플리케이션 기본 경로를 확인할 수 없습니다.");
                }

                var appSettingsPath = Path.Combine(basePath, "C:\\Source\\WAssistant\\ChatOrg");
                var directoryPath = Path.GetDirectoryName(appSettingsPath);
                if (string.IsNullOrEmpty(directoryPath))
                {
                    throw new InvalidOperationException("설정 파일 디렉토리를 확인할 수 없습니다.");
                }
                
                Configuration = new ConfigurationBuilder()
                    .SetBasePath("C:\\Source\\WAssistant\\ChatOrg")
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                _antapiKey = Configuration?["ApiKeys:AntapiKey"] ?? throw new InvalidOperationException("AntapiKey is not configured in appsettings.json");
                _openAIapiKey = Configuration?["ApiKeys:OpenAIapiKey"] ?? throw new InvalidOperationException("OpenAIapiKey is not configured in appsettings.json");
            }
            catch (Exception ex)
            {
                // 로깅 또는 사용자에게 오류 표시
                try
                {
                    // UI 스레드에서 MessageBox 표시
                    System.Windows.Application.Current?.Dispatcher.Invoke(() =>
                    {
                        System.Windows.MessageBox.Show(
                            $"설정 파일을 로드하는 중 오류가 발생했습니다: {ex.Message}",
                            "오류",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                    });
                }
                catch (Exception msgEx)
                {
                    // MessageBox가 실패한 경우 콘솔에 출력
                    Console.Error.WriteLine($"설정 파일 로드 오류: {ex.Message}\n메시지 박스 표시 중 오류: {msgEx.Message}");
                }
                throw;
            }
        }

        // 시스템 프롬프트(룰)
        static readonly string systemPrompt = 
@"너는 프레스 장비 점검 체크리스트를 관리하는 AI야.

사용자가 '공압 접검을 추가해줘', '선 점검을 추가해줘', '축 움직임과 모션이 디버링 박스 기준 우측 100mm 가는지 점검해줘'와 같이 요청하면,  
요청 내용을 간결하고 명확한 한국어로 정리해서 체크리스트에 들어갈 '점검 항목' 형태로 변환해줘.

- 반드시 핵심만 요약해서 한 줄로 작성해.
- 불필요한 설명이나 부연 없이, 바로 체크리스트에 쓸 수 있는 문장으로 만들어.
- 요청이 여러 개라면, 각 항목을 번호나 리스트로 구분해서 정리해.
- 전문용어는 정확하게 사용하고, 모호한 표현은 명확하게 바꿔줘.
- 점검 항목 이외의 정보는 포함하지 마.
- OpenAI 프롬프트 작성 가이드라인을 준수해.

```
예시)
입력: '공압 접검을 추가해줘'
출력: '공압 접점 상태 점검'

입력: '축 움직임과 모션이 디버링 박스 기준 우측 100mm 가는지 점검해줘'
출력: '축 모션이 디버링 박스 기준 우측 100mm 이동 여부 점검'

입력: '선 점검을 추가해줘, 공압 접검을 추가해줘'
출력:
1. 선 상태 점검
2. 공압 접점 상태 점검
```
";

        public static async Task<string> SendMessageAsync(string userInput, string previousResponseId = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAIapiKey}");

                var payload = new
                {
                    model = "gpt-4o",
                    input = systemPrompt + userInput, // 시스템 프롬프트 + 사용자 입력
                    previous_response_id = previousResponseId
                };

                var jsonPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    var output = doc.RootElement.GetProperty("output")[0]
                        .GetProperty("content")[0]
                        .GetProperty("text")
                        .GetString();
                    var id = doc.RootElement.GetProperty("id").GetString();
                    Console.WriteLine($"응답 ID: {id}");
                    return output;
                }
            }
        }

        public static async Task<string> SendMessageAsync(string[][] messages)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", _antapiKey);
                client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                // messages: [["user", "안녕? 나는 철수야."], ["assistant", "안녕, 철수!"], ["user", "내 이름이 뭐라고 했지?"]]
                var msgList = new System.Collections.Generic.List<object>();
                foreach (var msg in messages)
                {
                    msgList.Add(new { role = msg[0], content = msg[1] });
                }

                var payload = new
                {
                    model = "claude-3-opus-20240229",
                    max_tokens = 1024,
                    messages = msgList
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    var output = doc.RootElement.GetProperty("content")[0]
                        .GetProperty("text")
                        .GetString();
                    return output ?? string.Empty;
                }
            }
        }

        public static async Task<string> SendMessageAsync(string userInput)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", _antapiKey);
                client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                var msgList = new System.Collections.Generic.List<object>
                {
                    new { role = "user", content = systemPrompt },
                    new { role = "user", content = userInput }
                };

                var payload = new
                {
                    model = "claude-sonnet-4-20250514",
                    max_tokens = 1024,
                    messages = msgList
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    var output = doc.RootElement.GetProperty("content")[0]
                        .GetProperty("text")
                        .GetString();
                    return output ?? string.Empty;
                }
            }
        }
    }
}
