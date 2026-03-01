package com.group_finity.mascot;
import java.io.FileWriter;
import java.io.IOException;

public class Communicator {

    public static void sendCommand(String command) {
        // communicate.txt should be one level up from where the jar is running 
        // (the shared folder of all Shimeji forms)
        try (FileWriter writer = new FileWriter("../communicate.txt", false)) {
            writer.write("COMMAND=" + command);
            writer.flush();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
