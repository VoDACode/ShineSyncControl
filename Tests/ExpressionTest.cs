using ShineSyncControl.Models.DB;

namespace ShineSyncControl.Tests
{
    public class ExpressionTest
    {
        private List<Device> devices = new List<Device>();
        private List<DeviceProperty> properties = new List<DeviceProperty>();
        private List<Expression> expressions = new List<Expression>();
        private List<Models.DB.Task> tasks = new List<Models.DB.Task>();
        private List<Models.DB.Action> actions = new List<Models.DB.Action>();
        private List<ActionTask> actionTasks = new List<ActionTask>();

        DeviceProperty lightLevelSencor_level;
        DeviceProperty motionSensor_action;

        public void Run()
        {
            Fill();
            DoAction();
        }

        public void Fill()
        {
            devices.Clear();
            properties.Clear();
            expressions.Clear();
            tasks.Clear();
            actions.Clear();

            #region Light level sonsor

            Device lightLevelSencor = new Device
            {
                Id = 1,
                IsActive = true,
                LastSync = DateTime.UtcNow,
                Name = "Light level sonsor",
                Type = "-"
            };
            lightLevelSencor_level = new DeviceProperty
            {
                Id = 1,
                DeviceId = lightLevelSencor.Id,
                Device = lightLevelSencor,
                IsReadOnly = true,
                PropertyLastSync = DateTime.UtcNow,
                PropertyName = "level",
                Type = PropertyType.Number,
                PropertyUnit = null
            };
            lightLevelSencor_level.SetValue(0);
            lightLevelSencor.Properties.Add(lightLevelSencor_level);
            properties.Add(lightLevelSencor_level);

            #endregion

            #region Motion Sensor

            Device motionSensor = new Device
            {
                Id = 2,
                IsActive = true,
                LastSync = DateTime.UtcNow,
                Name = "Motion Sensor",
                Type = "-"
            };
            motionSensor_action = new DeviceProperty
            {
                Id = 1,
                DeviceId = motionSensor.Id,
                Device = motionSensor,
                IsReadOnly = true,
                PropertyLastSync = DateTime.UtcNow,
                PropertyName = "action",
                Type = PropertyType.Boolean,
                PropertyUnit = null
            };
            motionSensor_action.SetValue(false);
            motionSensor.Properties.Add(motionSensor_action);
            properties.Add(motionSensor_action);

            #endregion

            #region Relay

            Device relay = new Device
            {
                Id = 3,
                IsActive = true,
                LastSync = DateTime.UtcNow,
                Name = "Relay",
                Type = "-"
            };
            DeviceProperty relay_status = new DeviceProperty
            {
                Id = 1,
                DeviceId = motionSensor.Id,
                Device = motionSensor,
                IsReadOnly = false,
                PropertyLastSync = DateTime.UtcNow,
                PropertyName = "status",
                Type = PropertyType.Boolean,
                PropertyUnit = null
            };
            relay_status.SetValue(false);
            relay.Properties.Add(relay_status);
            properties.Add(relay_status);

            #endregion

            devices.Add(lightLevelSencor);
            devices.Add(motionSensor);
            devices.Add(relay);

            /*
                Expression:
                if lightLevelSencor_level <= 50 && motionSensor_action == true:
                    relay_status = true
                else:
                    relay_status = false
             */

            Expression lightLevelSencor_level_expression = new Expression
            {
                Id = 0,
                Device = lightLevelSencor,
                DeviceProperty = lightLevelSencor_level,
                Operator = Operator.LessThanOrEqual,
                Type = PropertyType.Number,
                Value = "50",

                ExpressionOperator = ExpressionOperator.And
            };

            Expression motionSensor_action_expression = new Expression
            {
                Id = 1,
                Device = motionSensor,
                DeviceProperty = motionSensor_action,
                Operator = Operator.Equal,
                Type = PropertyType.Boolean,
                Value = "true"
            };

            lightLevelSencor_level_expression.SubExpression = motionSensor_action_expression;
            expressions.Add(lightLevelSencor_level_expression);
            expressions.Add(motionSensor_action_expression);

            // tasks

            Models.DB.Task relay_status_false = new Models.DB.Task
            {
                Id = 0,
                Device = relay,
                DeviceId = relay.Id,
                DeviceProperty = relay_status,
                DevicePropertyId = relay.Id,
                Event = "set",
                Value = "false",
                Type = PropertyType.Boolean,
                Name = "Set to false"
            };

            Models.DB.Task relay_status_true = new Models.DB.Task
            {
                Id = 0,
                Device = relay,
                DeviceId = relay.Id,
                DeviceProperty = relay_status,
                DevicePropertyId = relay.Id,
                Event = "set",
                Value = "true",
                Type = PropertyType.Boolean,
                Name = "Set to true"
            };

            tasks.Add(relay_status_true);
            tasks.Add(relay_status_false);

            // action

            Models.DB.Action autoLightAction = new Models.DB.Action
            {
                Id = 0,
                Expression = lightLevelSencor_level_expression,
                Name = "Auto light"
            };

            actions.Add(autoLightAction);

            ActionTask actionTask = new ActionTask
            {
                WhenTrueTask = relay_status_true,
                WhenFalseTask = relay_status_false,
                Action = autoLightAction
            };

            actionTasks.Add(actionTask);
        }
   
        public void DoAction()
        {
            Random random = new Random();
            bool flag = false;
            while (true)
            {
                foreach (var actionTask in actionTasks)
                {
                    var action = actionTask.Action;
                    if (action.Expression.Execute())
                    {
                        actionTask?.WhenTrueTask.Execute();
                    }
                    else
                    {
                        actionTask?.WhenFalseTask.Execute();
                    }
                }
                // random light level
                lightLevelSencor_level.SetValue(random.Next(30, 100));
                motionSensor_action.SetValue(flag);
                flag = !flag;
                Thread.Sleep(500);
            }
        }
    }
}
