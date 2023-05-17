import socket
import json
import threading
import messages

class Client:
    def __init__(self, socket, address):
        self.socket = socket
        self.address = address
        
    def close(self):
        self.socket.close()

    def receive(self, buffer_size=4096):
        data = b''
        while True:
            part = self.socket.recv(buffer_size)
            data += part
            if len(part) < buffer_size:
                break
        return data

class Server:
    def __init__(self, host, port):
        self.stop = False
        self.host = host
        self.port = port
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.bind((host, port))
        self.server_socket.listen(5)  # FIXME : remove arg on listen ?
        self.clients = []
        self.process = None
        
    def start_server(self):
        self.process = threading.Thread(target=self.accept_clients)
        self.process.start()

    def accept_clients(self):
        while not self.stop:
            client_socket, client_address = self.server_socket.accept()
            client = Client(client_socket, client_address)
            self.clients.append(client)
            print(f"Accepted connection from {client}")
            
        self.server_socket.close()

    def send(self, messages):
        for client, message in zip(self.clients, messages):
            data = message.encode()
            client.socket.sendall(data)
            
    def recive(self):
        messeges = []
        for client in self.clients:
            message = client.receive().decode()
            messeges.append(message)
        return messeges

    def stop_server(self):
        
        #for client in self.clients:
        #    client.close()
        self.clients = []
        self.stop = True
        #self.process.join()
        self.server_socket.close()
        
