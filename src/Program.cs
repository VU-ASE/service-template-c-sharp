using System;
using roverlib;
using QuickType;

using Rovercom = ProtobufMsgs;

public static class RoverService
{
    public static void run(Service service, ServiceConfiguration configuration)
    {
        //
        // Get configuration values
        //
        if (configuration == null)
        {
            throw new InvalidOperationException("Configuration cannot be accessed");
        }

        //
        // Access the service identity, who am I?
        //
        Roverlog.Info($"Hello world, a new Go service '{service.Name}' was born at version {service.Version}");

        //
        // Access the service configuration, to use runtime parameters
        //
        float tunableSpeed;
        try
        {
            tunableSpeed = configuration.GetFloatSafe("speed");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get configuration: {ex.Message}");
        }
        Roverlog.Info($"Fetched runtime configuration example tunable number: {tunableSpeed}");

        //
        // Reading from an input, to get data from other services (see service.yaml to understand the input name)
        //
        ReadStream readStream = service.GetReadStream("imaging", "path");
        if (readStream == null)
        {
            throw new InvalidOperationException("Failed to get read stream");
        }

        //
        // Writing to an output that other services can read (see service.yaml to understand the output name)
        //
        WriteStream writeStream = service.GetWriteStream("decision");
        if (writeStream == null)
        {
            throw new InvalidOperationException("Failed to create write stream 'decision'");
        }

        while (true)
        {
            //
            // Reading one message from the stream
            //
            var data = readStream.Read();
            if (data == null)
            {
                throw new InvalidOperationException("Failed to read from 'imaging' service");
            }

            // When did the imaging service create this message?
            ulong createdAt = data.Timestamp;
            Roverlog.Info($"Recieved message with timestamp: {createdAt}");

            // Get the imaging data
            var imagingData = data.CameraOutput;
            if (imagingData == null)
            {
                throw new InvalidOperationException("Message does not contain camera output. What did imaging do??");
            }
            Roverlog.Info($"Imaging service captured a {imagingData.Trajectory.Width} by {imagingData.Trajectory.Height} image");

            // Print the X and Y coordinates of the middle point of the track that Imaging has detected
            if (imagingData.Trajectory.Points.Count > 0)
            {
                Roverlog.Info($"The X: {imagingData.Trajectory.Points[0].X} and Y: {imagingData.Trajectory.Points[0].Y} values of the middle point of the track");
            }
            else
            {
                Roverlog.Info("imaging could didn't detect track edges. Is the Rover on the track?");
            }

            // This value holds the steering position that we want to pass to the servo (-1 = left, 0 = center, 1 = right)
            float steerPosition = -0.5f;

            // Initialize the message that we want to send to the actuator
            var actuatorMsg = new Rovercom.SensorOutput
            {
                Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), // milliseconds since epoch
                Status = 0,                                                        // all is well
                SensorId = 1,
                ControllerOutput = new Rovercom.ControllerOutput
                {
                    SteeringAngle = steerPosition,
                    LeftThrottle = tunableSpeed,
                    RightThrottle = tunableSpeed,
                    FanSpeed = 0,
                    FrontLights = false,
                }
            };

            // Send the message to the actuator
            try
            {
                writeStream.Write(actuatorMsg);
            }
            catch (Exception ex)
            {
                Roverlog.Warn($"Could not write to actuator: {ex.Message}");
            }

            //
            // Now do something else fun, see if our "tunable_speed" is updated
            //
            float curr = tunableSpeed;

            Roverlog.Info("Checking for tunable number update");

            // We are not using the safe version here, because using locks is boring
            // (this is perfectly fine if you are constantly polling the value)
            // nb: this is not a blocking call, it will return the last known value
            float newVal;
            try
            {
                newVal = configuration.GetFloat("speed");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get updated tunable number: {ex.Message}");
            }

            if (curr != newVal)
            {
                Roverlog.Info($"Tunable number updated: {curr} -> {newVal}");
                curr = newVal;
            }
            tunableSpeed = curr;
        }
    }

    // This function gets called when roverd wants to terminate the service
    public static void onTerminate(ConsoleSpecialKey signal)
    {
        Roverlog.Info($"Terminating service with signal: {signal.ToString()}");

        //
        // ...
        // Any clean up logic here
        // ...
        //
    }
}

// This is just a wrapper to run the user program
// it is not recommended to put any other logic here
class Program
{
    static void Main(string[] args)
    {
        Rover.Run(RoverService.run, RoverService.onTerminate);
    }
}