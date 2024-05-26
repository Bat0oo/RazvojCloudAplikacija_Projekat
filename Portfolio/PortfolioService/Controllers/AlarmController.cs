using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Alarm_Data;

namespace PortfolioService.Controllers
{
    public class AlarmController : ApiController
    {
        private readonly AlarmDataRepository _alarmDataRepository;

        public AlarmController(AlarmDataRepository alarmDataRepository)
        {
            _alarmDataRepository = alarmDataRepository ?? throw new ArgumentNullException(nameof(alarmDataRepository));
        }

        public AlarmController()
        {
            _alarmDataRepository = new AlarmDataRepository();
        }

        [HttpPost]
        [Route("api/alarm")]
        public IHttpActionResult AddAlarm([FromBody] Alarm newAlarm)
        {
            if (newAlarm == null)
            {
                return BadRequest("Invalid alarm data.");
            }

            Alarm alarm = new Alarm(Guid.NewGuid().ToString())
            {
                UserEmail = newAlarm.UserEmail,
                CryptoSymbol = newAlarm.CryptoSymbol,
                TargetPrice = newAlarm.TargetPrice,
                AboveOrBelow = newAlarm.AboveOrBelow,
                IsTriggered = newAlarm.IsTriggered
            };


            _alarmDataRepository.AddAlarm(alarm);
            return Ok("Alarm added successfully.");
        }

        [HttpGet]
        [Route("api/alarm")]
        public IHttpActionResult GetAlarms(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("User email is required.");
            }

            var alarms = _alarmDataRepository.GetAlarmsByUserEmail(userEmail);
            return Ok(alarms);
        }

        [HttpPut]
        [Route("api/alarm")]
        public IHttpActionResult UpdateAlarm([FromBody] Alarm updatedAlarm)
        {
            if (updatedAlarm == null)
            {
                return BadRequest("Invalid alarm data.");
            }

            _alarmDataRepository.UpdateAlarm(updatedAlarm);
            return Ok("Alarm updated successfully.");
        }

        [HttpDelete]
        [Route("api/alarm")]
        public IHttpActionResult DeleteAlarm(string userEmail, string alarmId)
        {
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(alarmId))
            {
                return BadRequest("User email and alarm ID are required.");
            }

            _alarmDataRepository.RemoveAlarm(userEmail, alarmId);
            return Ok("Alarm deleted successfully.");
        }
    }
}
