import win32file
import pywintypes
import time

def main():
    pipe_name = r'\\.\pipe\MTPP_Pipe'

    while True:
        try:
            handle = win32file.CreateFile(
                pipe_name,
                win32file.GENERIC_READ | win32file.GENERIC_WRITE,
                0,
                None,
                win32file.OPEN_EXISTING,
                0,
                None
            )
            break
        except pywintypes.error as e:
            if e.winerror == 2 or e.winerror == 231:
                time.sleep(0.05)
            else:
                raise

    try:
        with open('named_pipe_log_python.txt', 'a') as f:
            while True:
                err, raw_bytes = win32file.ReadFile(handle, 4)
                
                if err != 0 or len(raw_bytes) == 0:
                    break

                number = int.from_bytes(raw_bytes, byteorder='little', signed=True)
                
                f.write(f"Sub-thread int recieved back:{number};\n")
                f.flush()
                
                result = number * -1
                result_bytes = result.to_bytes(4, byteorder='little', signed=True)
                
                win32file.WriteFile(handle, result_bytes)
            
    finally:
        win32file.CloseHandle(handle)

if __name__ == "__main__":
    main()