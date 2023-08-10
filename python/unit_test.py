"""
Unit tests for all python components of this project
"""


import unittest
import socket
import time

import messages
from unity_env import dict_to_vector, obs_to_vect, UnityEnv
from server import Server, Client


class TestServer(unittest.TestCase):
    """
    Tests server communications
    """
    @classmethod
    def setUpClass(cls):
        for port in range(8080, 9000):
            try:
                cls.serv = Server("127.0.0.1", port)
            except OSError:
                pass
            else:
                break
        cls.serv.start_server()
        cls.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        cls.client.connect(("127.0.0.1", port))
        time.sleep(1)
    
    @classmethod
    def tearDownClass(cls):
        cls.serv.stop_server()
        cls.client.close()
        time.sleep(1)

    def test_send(self):
        self.serv.send("payload")
        data = self.client.recv(1024)
        self.assertEqual(data[:4], b'\x07\x00\x00\x00', "message framing")
        self.assertEqual(data[4:], b'payload', "message itself")

    def test_receive(self):
        self.client.sendall(b'\x07\x00\x00\x00payload+extra')
        data = self.serv.receive()
        self.assertEqual(data, "payload")

class TestMessages(unittest.TestCase):
    """
    Tests serialisation and deserialisation of messages
    """
    def test_request(self):
        msg = messages.RequestMessage("apiName", "param")
        json = msg.to_json()
        self.assertEqual(json, '{"api": "apiName", "parameter": "param"}')
        self.assertEqual(messages.RequestMessage.from_json(json), msg)

    def test_multi(self):
        sub_msg = [messages.ControllMessage(0, 0, i) for i in range(3)]
        msg = messages.MultiControllMessage(sub_msg)
        json = msg.to_json()
        self.assertEqual(json, '{"messages": [{"moveInput": 0.0, "turnInput": 0.0, "envNum": 0}, {"moveInput": 0.0, "turnInput": 0.0, "envNum": 1}, {"moveInput": 0.0, "turnInput": 0.0, "envNum": 2}]}')
        self.assertEqual(messages.MultiControllMessage.from_json(json), msg)


class TestEnvironment(unittest.TestCase):
    """
    Tests environment interactions
    """
    @classmethod
    def setUpClass(cls):
        for port in range(8080, 9000):
            try:
                cls.serv = Server("127.0.0.1", port)
            except OSError:
                pass
            else:
                break
        cls.serv.start_server()
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.connect(("127.0.0.1", port))
        cls.client = Client(sock, '')

        cls.env = UnityEnv(cls.serv, 2, 2, 1, lambda : (0,0), "unit_test_env")
        cls.multi_env = UnityEnv(cls.serv, 2, 2, 1, lambda : (0,0), "unit_test_env", agent_number=2)

        cls.dummy_obs = messages.ObservationMessage(*([dict(x=0, y=0, z=0)]*9), *([0]*11))
        time.sleep(1)
    
    @classmethod
    def tearDownClass(cls):
        cls.serv.stop_server()
        cls.client.close()
        time.sleep(1)

    def test_dict_to_vector(self):
        """
        Test that n dict can be vectorized
        """
        d = dict(x=420, y=25, z=100)
        vect = dict_to_vector(d, ('x', 'y'), norm=10)
        self.assertEqual(vect, [42.0, 2.5])
        
    def test_simple_step(self):
        """
        Tests single step sends the correct requests to Unity env
        """
        resp = messages.ResponseMessage(self.dummy_obs.to_json())
        self.client.send(resp.to_json().encode())  # send response before stepping since step will wait for response
        self.env.step((1, 2), envNum=666)
        req = messages.RequestMessage.from_json(self.client.receive())
        act = messages.ControllMessage.from_json(req.parameter)

        self.assertEqual(act, messages.ControllMessage(1, 2, 666))
        self.assertEqual(req.api, "step")

    def test_simple_reset(self):
        """
        Tests single reset sends the correct requests to Unity env
        """
        resp = messages.ResponseMessage(self.dummy_obs.to_json())
        self.client.send(resp.to_json().encode())  # send response before stepping since step will wait for response
        self.env.reset(2)
        req = messages.RequestMessage.from_json(self.client.receive())
        act = messages.SingleFieldMessage.from_json(req.parameter)

        self.assertEqual(act, messages.SingleFieldMessage(2))
        self.assertEqual(req.api, "reset")

    def test_multi_step(self):
        """
        Tests multi step sends the correct requests to Unity env
        """
        resp = messages.ResponseMessage(messages.MultiObservationMessage((self.dummy_obs, self.dummy_obs)).to_json())
        self.client.send(resp.to_json().encode())  # send response before stepping since step will wait for response
        self.env.step(((1, 2), (3, 4)))
        req = messages.RequestMessage.from_json(self.client.receive())
        act = messages.MultiControllMessage.from_json(req.parameter)

        self.assertEqual(act, messages.MultiControllMessage((messages.ControllMessage(1, 2, 0), messages.ControllMessage(3, 4, 1))))
        self.assertEqual(req.api, "multiStep")
        
    def test_reset_step(self):
        """
        Tests multi reset sends the correct requests to Unity env
        """
        resp = messages.ResponseMessage(messages.MultiObservationMessage((self.dummy_obs, self.dummy_obs)).to_json())
        self.client.send(resp.to_json().encode())  # send response before stepping since step will wait for response
        self.env.reset([1, 0, 1])
        req = messages.RequestMessage.from_json(self.client.receive())
        act = messages.MultiMessage.from_json(req.parameter)

        self.assertEqual(act, messages.MultiMessage([0, 2]))
        self.assertEqual(req.api, "multiStep")


if __name__ == '__main__':
    unittest.main()
