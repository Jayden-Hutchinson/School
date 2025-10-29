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

            String line;
            String reply;

            while (true) {
                System.out.print("> ");
                if ((line = stdin.readLine()) == null) {
                    break;
                }
                out.println(line);
                if ((reply = in.readLine()) == null) {
                    break;
                }
                System.out.println(reply);
            }
        }
    }
}