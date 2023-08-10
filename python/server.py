import socket
import json
import threading
import messages

class Client:
    """
    A Client holding the communication socket with the unity side, and implementing message framing protocole
    """
    def __init__(self, socket, address):
        self.socket = socket
        self.address = address
        self.socket.settimeout(1)
        
    def close(self):
        self.socket.close()

    def receive(self):
        """
        A function that blocks until it receives data from the socket, and returns the full data once received
        """
        data = b''
        lenBuffer = b''
        lenBufferLen = 4

        while len(lenBuffer) != lenBufferLen:
            lenBuffer+= self.socket.recv(lenBufferLen-len(lenBuffer))
        msgLen = int.from_bytes(lenBuffer, 'little', signed=False)

        while len(data) != msgLen:
            data += self.socket.recv(msgLen-len(data))

        return data
    
    def send(self, data):
        """
        Sends data after framing it
        
        data -- unframed byte array to send
        """
        data = (len(data).to_bytes(4, 'little', signed=False)) + data
        self.socket.sendall(data)


class Server:
    """
    A server holding the server socket, handles communications with Unity side.
    Can only accept one client at a time
    """
    def __init__(self, host, port):
        self.stop = False
        self.host = host
        self.port = port
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        try:
            self.server_socket.bind((host, port))
        except OSError as err:
            self.server_socket.close()
            raise err

        self.server_socket.listen()
        self.server_socket.settimeout(1)

        self.client = None
        self.process = None
        
    def start_server(self):
        self.process = threading.Thread(target=self.accept_clients)
        self.process.start()

    def accept_clients(self):
        """
        listen for connections and accept any incoming client, until explicitly stoped
        """
        while not self.stop:
            try:
                client_socket, client_address = self.server_socket.accept()
                client = Client(client_socket, client_address)
                self.client = client
                print(f"Accepted connection from {client} ({client_address})")
            except TimeoutError:
                pass

        self.server_socket.close()

    def send(self, message):
        """
        Send data to the client
        
        message -- unframed string message to send
        """
        self.client.send(message.encode())
            
    def receive(self):
        """
        returns unframed string representation of the received data
        """
        return self.client.receive().decode()

    def stop_server(self):
        """
        stops the listening for incoming connections and close the connection with the current client
        """
        self.stop = True
        if self.client:
            self.client.socket.close()
        self.client = None
