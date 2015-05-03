//
//  ViewController.swift
//  LunarTestApp
//
//  Created by Alex Lementuev on 1/18/15.
//  Copyright (c) 2015 Space Madness. All rights reserved.
//

import UIKit

class ViewController: UIViewController, UITextFieldDelegate {

    private let kKeyHostField = "host"
    private let kKeyPortField = "port"
    
    private var netPeer: LunarNetClientPeer!
    private var netTimer: NSTimer!
    
    @IBOutlet weak var hostField: UITextField!
    @IBOutlet weak var portField: UITextField!
    @IBOutlet weak var connectButton: UIButton!
    @IBOutlet weak var sendButton: UIButton!
    @IBOutlet weak var statusLabel: UILabel!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.

        hostField.delegate = self
        portField.delegate = self
        
        let defaults = NSUserDefaults.standardUserDefaults()
        
        if let host = defaults.stringForKey(kKeyHostField) {
            hostField.text = host
        }
        
        if let port = defaults.stringForKey(kKeyPortField) {
            portField.text = port
        }
        
        sendButton.enabled = false
        connectButton.addTarget(self, action: "connect:", forControlEvents: .TouchUpInside)
        
        let configuration = LunarNetConfiguration(appId: "test")
        netPeer = LunarNetClientPeer(configuration: configuration)
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    func connect(sender: AnyObject)
    {
        let host = hostField.text
        let port = portField.text
        
        let defaults = NSUserDefaults.standardUserDefaults()
        defaults.setObject(host, forKey: kKeyHostField)
        defaults.setObject(port, forKey: kKeyPortField)
        defaults.synchronize()
        
        netPeer.start()
        netPeer.sendDiscoveryRequest()

        netTimer = NSTimer.scheduledTimerWithTimeInterval(0.2, target: self, selector: "netTimerCallback", userInfo: nil, repeats: true)

    }
    
    func disconnect(sender: AnyObject)
    {
        netPeer.stop()
    }
    
    @IBAction func showAssert(sender: AnyObject)
    {
        let assertController = LunarAssertViewController(title: "Assert")
        presentViewController(assertController, animated: true, completion: nil);
    }
    
    func netTimerCallback()
    {
        while let msg = netPeer.readMessage() {
            switch msg.messageType
            {
            case .StatusChanged:
                var statusByte: CChar = 0
                if msg.readInt8(&statusByte)
                {
                    let status = LUNetConnectionStatus(rawValue: CUnsignedLong(statusByte))!
                    switch status
                    {
                    case .Connected:
                        onConnected()
                        break;
                    case .Disconnected:
                        onDisconnected()
                        break;
                    default:
                        break;
                    }
                }
                break;
                
            case .Data:
                var data: NSString?;
                if msg.readString(&data)
                {
                    println(data!)
                }
                break;
                
            case .DiscoveryResponse:
                println("Discovery response");
                break;
                
            default:
                break;
            }
        }
    }
    
    @IBAction func onSendButton(sender: AnyObject) {
        for var i: Int = 0; i < 5; ++i
        {
            let msg: LunarNetMessage! = LunarNetMessage(type: .Data)
            msg.writeString("Testing - \(i)");
            netPeer.sendMessage(msg)
        }
    }
    
    func onConnected()
    {
        sendButton.enabled = true
        
        connectButton.removeTarget(self, action: "connect:", forControlEvents: .TouchUpInside)
        connectButton.addTarget(self, action: "disconnect:", forControlEvents: .TouchUpInside)
        
        connectButton.titleLabel?.text = "Disconnect"
    }
    
    func onDisconnected()
    {
        sendButton.enabled = false
        
        connectButton.removeTarget(self, action: "connect:", forControlEvents: .TouchUpInside)
        connectButton.addTarget(self, action: "disconnect", forControlEvents: .TouchUpInside)
        
        connectButton.titleLabel?.text = "Connect"
    }
    
    func textFieldShouldReturn(textField: UITextField) -> Bool
    {
        textField.resignFirstResponder()
        return false
    }
}

