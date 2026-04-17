import socket
import time

def main():
    HOST = '127.0.0.1'
    PORT = 5050

    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    while True:
        try:
            s.connect((HOST, PORT))
            break
        except ConnectionRefusedError:
            time.sleep(0.05)

    try:
        with open('socket_log_python.txt', 'a') as f:
            while True:
                raw_bytes = s.recv(4)
            
                if not raw_bytes:
                    break

                number = int.from_bytes(raw_bytes, byteorder='little', signed=True)
            
                f.write(f"Sub-thread received: {number}\n")
                f.flush()
            
                result = number * -1
                result_bytes = result.to_bytes(4, byteorder='little', signed=True)
            
                s.sendall(result_bytes)
            
    finally:
        s.close()

if __name__ == "__main__":
    main()