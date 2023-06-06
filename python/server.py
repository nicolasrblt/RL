import socket
import json
import threading
import messages

class Client:
    def __init__(self, socket, address):
        self.socket = socket
        self.address = address
        self.socket.settimeout(1)
        
    def close(self):
        self.socket.close()

    def receive(self):
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
        data = (len(data).to_bytes(4, 'little', signed=False)) + data
        self.socket.sendall(data)


class Server:
    def __init__(self, host, port):
        self.stop = False
        self.host = host
        self.port = port
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.bind((host, port))
        self.server_socket.listen()
        self.server_socket.settimeout(1)

        self.client = None
        self.process = None
        
    def start_server(self):
        self.process = threading.Thread(target=self.accept_clients)
        self.process.start()

    def accept_clients(self):
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
        self.client.send(message.encode())
            
    def receive(self):
        return self.client.receive().decode()

    def stop_server(self):
        self.stop = True
        if self.client:
            self.client.socket.close()
        self.client = None
