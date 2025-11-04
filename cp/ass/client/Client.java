package client;

import java.io.*;
import java.net.*;

class Client {
    public static void main(String... args) throws IOException {
        final String host = "127.0.0.1";
        final int port = 6666;
        try (
                final Socket s = new Socket(host, port);
                final BufferedReader in = new BufferedReader(new InputStreamReader(s.getInputStream()));
                final PrintWriter out = new PrintWriter(new OutputStreamWriter(s.getOutputStream()), true);
                final BufferedReader stdin = new BufferedReader(new InputStreamReader(System.in));) {

            Thread reader = new Thread(() -> {
                String reply;
                try {
                    while ((reply = in.readLine()) != null) {
                        System.out.println(reply);
                        System.out.flush();
                    }
                } catch (IOException error) {
                    System.out.println("Connection closed.");
                }
            });
            reader.setDaemon(true);
            reader.start();

            String line;
            while (true) {
                line = stdin.readLine();
                if (line == null) {
                    break;
                }
                out.println(line);
            }
        }
    }
}