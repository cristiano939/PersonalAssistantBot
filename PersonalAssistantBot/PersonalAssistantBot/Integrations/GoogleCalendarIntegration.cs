using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PersonalAssistantBot.Integrations
{
    class GoogleCalendarIntegration
    {


        static string[] Scopes = { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar };
        static string ApplicationName = "PersonalAssistant";

        public Events ReadAllEvents()
        {
            var service = CreateService();
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            return events;
        }

        private CalendarService CreateService()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }

        public Event SetEventOnCalendar()
        {
            var service = CreateService();
            Event rhEvent = new Event();

            rhEvent.Summary = "Testing Event";
            rhEvent.Location = "TakeNet Prado";
            rhEvent.Description = "Reunião com a josislaine";
            rhEvent.Start = new EventDateTime { DateTime = new DateTime(2017, 01, 09, 19, 00, 00),TimeZone = "America/Sao_Paulo" };
            rhEvent.End = new EventDateTime { DateTime = new DateTime(2017, 01, 09, 20, 00, 00), TimeZone = "America/Sao_Paulo" };

            rhEvent.Recurrence = new List<string> { "RRULE:FREQ=DAILY;COUNT=2" };
            rhEvent.Attendees = new List<EventAttendee> {
                new EventAttendee {Email="cristianog@take.net",DisplayName="Keyla Margarete",Organizer=true },
                new EventAttendee {Email="cristianog@take.net",DisplayName="Tião",Organizer= true,Self=true }
            };

            EventReminder[] reminder = new EventReminder[1];
            reminder[0] = new EventReminder { Minutes = 10, Method = "email" };

            rhEvent.Reminders = new Event.RemindersData { Overrides = reminder.ToList(),UseDefault = false };
            try
            {
                rhEvent = service.Events.Insert(rhEvent, "primary").Execute();
            }
            catch (Exception ex)
            {

                
            }
            return rhEvent;
        }

    }
}

