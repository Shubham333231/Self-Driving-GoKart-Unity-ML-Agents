# 🏎️ Self-Driving Go-Kart using Unity ML-Agents

![ML](https://img.shields.io/badge/ML-Reinforcement%20Learning-blue)
![Unity](https://img.shields.io/badge/Unity-ML--Agents-black)
![Python](https://img.shields.io/badge/Python-3.8%2B-green)
![Algorithm](https://img.shields.io/badge/Algorithm-PPO-orange)
![Status](https://img.shields.io/badge/Status-Completed-brightgreen)

> A self-driving Go-Kart simulation trained using Reinforcement Learning
> via Unity ML-Agents framework with Proximal Policy Optimization (PPO).

---

## 🎯 Project Overview

This project implements an autonomous Go-Kart agent that learns to navigate
a racing track without any human input. The agent is trained using
**Proximal Policy Optimization (PPO)** — a state-of-the-art reinforcement
learning algorithm used in real-world robotics and autonomous systems.

The agent starts with zero knowledge and learns entirely through
**trial and error** using reward signals — penalized for collisions and
going off-track, rewarded for forward progress and checkpoint completion.

---

## 🧠 ML Concepts Applied

| Concept | Implementation |
|--------|---------------|
| Reinforcement Learning | Agent-environment interaction loop |
| PPO Algorithm | Policy gradient optimization |
| Reward Engineering | Custom reward/penalty shaping |
| Neural Network Policy | 2-layer MLP (256 hidden units) |
| Observation Space | Velocity, direction, distance (13 obs) |
| Action Space | Continuous: steering, acceleration, braking |
| Exploration vs Exploitation | Entropy regularization (beta=0.005) |
| Generalized Advantage Estimation | Lambda=0.95 |

---

## ⚙️ Tech Stack

| Technology | Version | Purpose |
|-----------|---------|---------|
| Unity3D | 2021.3+ | Simulation Environment |
| Unity ML-Agents | 0.30.0 | RL Training Framework |
| Python | 3.8+ | Training Configuration & Scripts |
| PyTorch | 1.8+ | Neural Network Backend |
| C# | — | Agent & Environment Logic |
| TensorBoard | 2.6+ | Training Visualization |

---

## 📁 Project Structure

```
Self-Driving-GoKart-Unity-ML-Agents/
│
├── Scripts/
│   ├── GoKartAgent.cs          # Main RL agent (observations, actions, rewards)
│   └── TrackCheckpoint.cs      # Checkpoint trigger system
│
├── Config/
│   └── GoKart.yaml             # PPO hyperparameter configuration
│
├── requirements.txt            # Python dependencies
└── README.md                   # Project documentation
```

---

## 🔁 System Architecture

```
┌─────────────────────────────────────────────────────┐
│                  TRAINING LOOP                       │
│                                                      │
│  Environment ──→ Observations ──→ Neural Network    │
│      ↑               (13)         Policy (PPO)      │
│      │                                  ↓           │
│  Reward Signal ←── Unity ←──── Actions (3)          │
│  (checkpoint,      Engine      (steer, accel,       │
│   collision,                    brake)              │
│   progress)                                         │
└─────────────────────────────────────────────────────┘
```

---

## 🎁 Reward Structure

| Event | Reward |
|-------|--------|
| Moving toward checkpoint | +0.1 per unit |
| Checkpoint reached | +1.0 |
| Lap completed | +5.0 |
| Wall collision | -0.5 |
| Going off track | -1.0 (episode ends) |
| Being stationary | -0.01 per step |

---

## 📊 Training Configuration (GoKart.yaml)

```yaml
trainer_type: ppo
hyperparameters:
  learning_rate: 3.0e-4
  batch_size: 128
  buffer_size: 2048
  beta: 5.0e-3        # Entropy (exploration)
  epsilon: 0.2        # PPO clip range
  lambd: 0.95         # GAE lambda
  num_epoch: 3
network_settings:
  hidden_units: 256
  num_layers: 2
  normalize: true
max_steps: 500000
```

---

## 🚀 How to Run

### Prerequisites
```bash
# Install Python dependencies
pip install -r requirements.txt
```

### Unity Setup
1. Install **Unity 2021.3+**
2. Add **ML-Agents package** via Unity Package Manager
3. Open project and assign scripts to Go-Kart GameObject
4. Tag track objects: `Wall`, `Checkpoint`, `OffTrack`, `Track`

### Start Training
```bash
mlagents-learn Config/GoKart.yaml --run-id=GoKart_Run1
```

### Monitor Training (TensorBoard)
```bash
tensorboard --logdir results
```

### Test Trained Agent
- Load saved model from `results/GoKart_Run1/`
- Set Agent Behavior Type to **Inference** in Unity Inspector
- Press Play in Unity Editor

---

## 📈 Results

- ✅ Agent successfully learned autonomous track navigation
- ✅ Zero collision laps achieved after full training
- ✅ Reward convergence observed within 500,000 steps
- ✅ Smooth continuous steering and speed control developed
- ✅ Consistent lap completion without human input

---

## 📚 Reference

Project based on architecture and concepts by **Fabrizio Frigeni**
([Udemy – Self-Driving Go-Kart with Unity ML-Agents](https://www.udemy.com))

---

## 👤 Author

**Shubham Sharma**
B.Tech Electronics and Communication Engineering
Matoshri College of Engineering and Research Centre, Nashik
Savitribai Phule Pune University | CGPA: 7.91

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Connect-blue)](https://linkedin.com/in/shubham-sharma-192b32341)

---

## 📄 License

This project is for educational purposes only.
